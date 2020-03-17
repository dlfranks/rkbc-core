//A Grid view model for editable rows
ko.GridViewModel = function (config) {
    var self = this;
    self.options = $.extend({
        pagingEnabled: true,
        currentPage: 0,
        pageSize: 10,
        ctor: undefined,         //constructor to be used for construction returned items
        indexUrl: undefined,     //url {pageSize},{start} gets appended
        getUrl: undefined,       //url to return a default new object, still has ctor applied
        sortable: [],
        reportLoadErrors: true   //jGrowl "Exception unable to load page...", for legacy pages, really this should always be false.
    }, config);

    var SortableHeaderViewModel = function (key, dir) {
        var sortself = this;
        sortself.key = ko.observable(key);
        sortself.dir = ko.observable(dir || 0);
        sortself.click = function () {
            var tmp = sortself.dir();
            self.clearAllSort();

            //Go from descending to cleared (we only allow one right now, so this does not make sense)
            //if (tmp == -1) sortself.dir(0);
            //Ascending to descending
            if (tmp == 1) sortself.dir(-1);
            //Nothing to ascending
            else sortself.dir(1);

            if (self.confirmNavigationAlert()) {
                self.currentPage(0);
                self.loadPage();
            }
        };
        sortself.indicator = ko.computed(function () {
            if (this.dir() == 0) return 'glyphicon';
            if (this.dir() == 1) return 'glyphicon glyphicon-chevron-up';
            if (this.dir() == -1) return 'glyphicon glyphicon-chevron-down';
        }, sortself);
    };
    self.clearAllSort = function () {
        for (var k in self.sort) { self.sort[k].dir(0); }
    };
    self.sortToParam = function () {
        var fin = { sortkey: '', sortdir: '' };
        for (var k in self.sort) {
            if (self.sort[k].dir() !== 0) {
                fin.sortkey = k;
                fin.sortdir = self.sort[k].dir();
            };
        }
        return (fin);
    };

    //Setup the items
    self.currentPage = ko.observable(self.options.currentPage);
    self.pageSize = ko.observable(self.options.pageSize);
    self.itemServerCount = ko.observable(0);
    self.items = ko.observableArray([]);
    self.sort = {};
    for (var k in self.options.sortable) { self.sort[k] = new SortableHeaderViewModel(k, self.options.sortable[k].dir); }

    self.initializeData = function (data, itemServerCount) {
        self.currentPage(0);
        self.itemServerCount(itemServerCount);
        if (data && data.length && self.options.ctor !== null) {
            data = $.map(data, function (el) {
                return new self.options.ctor(self, el);
            });
        }
        self.items([]);
        if (data && data.length) {
            self.items(data);
        }
    };

    self.disableAnimation = ko.observable(true);
    self.afterAddItem = function (elem) {
        if (!self.disableAnimation() && elem.nodeType === 1) $(elem).hide().slideDown()
    };
    self.beforeRemoveItem = function (elem) {
        if (elem.nodeType === 1) {
            if (!self.disableAnimation()) $(elem).slideUp(function () { $(elem).remove(); })
            else $(elem).remove();
        }
    };

    self.prevPage = function () {
        if (self.prevPageEnabled() && self.confirmNavigationAlert()) {
            self.currentPage(self.currentPage() - 1);
            self.loadPage();
        }
    };
    self.prevPageEnabled = function () {
        return (self.currentPage() > 0 && self.options.pagingEnabled);
    };
    self.nextPage = function () {
        if (self.nextPageEnabled() && self.confirmNavigationAlert()) {
            self.currentPage(self.currentPage() + 1);
            self.loadPage();
        }
    };
    self.nextPageEnabled = function () {
        return (self.currentPage() * self.pageSize() + self.pageSize() < self.itemServerCount()) && self.options.pagingEnabled;
    };

    self.loadRange = function (start, count) {
        //$('body').css('cursor', 'progress');
        $('body').addClass('wait');
        var promise = $.ajax({
            url: self.options.indexUrl + "?getStartRecord=" + start +
                "&getRecordCount=" + count +
                "&currentPage=" + self.currentPage() +
                "&currentPageSize=" + self.pageSize() +
                "&" + $.param(self.filter ? self.filter() : {}) +
                "&" + $.param(self.sortToParam()),
            type: 'POST',
            dataType: 'json',
            data: {},
            contentType: 'application/json; charset=utf-8',
            timeout: 60 * 1000 * 2  //2 minutes to retrieve
        })
            .fail(function (xhr, textStatus, errorThrown) {
                if (self.options.reportLoadErrors) {
                    $.jGrowl('<p class="error">Exception, unable to load records. ' + errorThrown + '</p>', { life: 'sticky' });
                }
            })
            .always(function () {
                //$('body').css('cursor', 'default');
                $('body').removeClass('wait');
            })
            .then(function (data) {
                if (data && data.items && data.items.length && self.options.ctor !== null) {
                    data.items = $.map(data.items, function (el) {
                        return new self.options.ctor(self, el);
                    });
                }
                return (data);
            });

        return (promise);
    };

    self.loadPage = function () {
        var promise = self.loadRange(self.currentPage() * self.pageSize(), self.pageSize())
            .then(function (data) {
                self.items([]);
                self.itemServerCount(0);
                if (data && data.items && data.items.length) {
                    //Duplicates are displayed for a time when animation is enabled
                    self.items(data.items);
                    self.itemServerCount(data.itemServerCount);
                }
            });
        return (promise);
    };

    self.deleteItem = function (item) {
        var index = self.items.indexOf(item);
        if (index >= 0) {
            self.disableAnimation(false);
            self.items.remove(item);
            self.disableAnimation(true);
            if (self.options.pagingEnabled) {
                //If that was the last item, drop to a prior page
                if (self.items().length == 0) {
                    self.currentPage(self.currentPage() ? self.currentPage() - 1 : 0);
                    self.loadPage();
                }
            }
        }
    };

    //When you post and change stuff server side, it is easier to just reload the item sometimes
    self.replaceItem = function (oldItem, newItem) {
        var index = self.items.indexOf(item);
        if (index >= 0) {
            self.disableAnimation(false);
            self.items.splice(index, 1, newItem);
            self.disableAnimation(true);
        }
    };

    self.refreshItem = function (item) {
        if (!item || !item.id()) throw "Cannot refresh a new or non-existent item.";
        //$('body').css('cursor', 'progress');
        $('body').addClass('wait');
        var promise = $.ajax({
            url: self.options.getUrl + (self.options.getUrl.indexOf("?") >= 0 ? "&id=" : "?id=") + item.id(),
            type: 'POST',
            dataType: 'json',
            data: {},
            contentType: 'application/json; charset=utf-8'
        })
            .fail(function (xhr, textStatus, errorThrown) {
                $.jGrowl('<p class="error">Exception, unable refresh item. ' + errorThrown + '</p>', { life: 'sticky' });
            })
            .always(function () {
                //$('body').css('cursor', 'default');
                $('body').removeClass('wait');
            })
            .then(function (data) {
                if (data) {
                    if (self.options.ctor !== null) {
                        data = new self.options.ctor(self, data);
                    }
                    //Make sure the item still exists and get its new index (case anybody went crazy during load)
                    index = self.items.indexOf(item);
                    if (index >= 0) self.items.splice(index, 1, data);
                }
                return (data);
            });

        return (promise);
    };

    self.addItem = function (item, toBottom) {
        self.disableAnimation(false);
        //Assumes toBottom true or false is set based on where the button was clicked, so no scrolling should be necessary
        if (toBottom) {
            //Put the new item at the bottom
            self.items.push(item);
        } else {
            //Put the new item at the top by default
            self.items.splice(0, 0, item);
        }
        self.disableAnimation(true);
    };

    self.newItem = function (data, event, toBottom) {
        //$('body').css('cursor', 'progress');
        $('body').addClass('wait');
        var promise = $.ajax({
            url: self.options.getUrl,
            type: 'POST',
            dataType: 'json',
            data: {},
            contentType: 'application/json; charset=utf-8'
        })
            .fail(function (xhr, textStatus, errorThrown) {
                $.jGrowl('<p class="error">Exception, unable to create a new record. ' + errorThrown + '</p>', { life: 'sticky' });
            })
            .always(function () {
                //$('body').css('cursor', 'default');
                $('body').removeClass('wait');
            })
            .then(function (data) {
                if (data) {
                    if (self.options.ctor !== null) {
                        data = new self.options.ctor(self, data);
                    }
                    self.addItem(data, toBottom);
                }
                return (data);
            });

        return (promise);
    };

    self.confirmNavigation = function () {
        var isDirty = false;
        for (var i = 0; i < self.items().length; i++) {
            if (self.items()[i].dirtyFlag && self.items()[i].dirtyFlag.isDirty()) {
                isDirty = true;
                break;
            }
        }
        if (isDirty) return "One or more records that was created or updated has not been saved.";
    };

    self.confirmNavigationAlert = function () {
        var dirty = self.confirmNavigation();
        return (!dirty || confirm("One or more records that was created or updated has not been saved. If you continue those changes will be lost."));
    };
};
