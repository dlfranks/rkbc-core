/* -- Data Panel Plugin ----------------------------------------------------------*/
/* rowLevelSubmit - false if we need to rename the fields for a full form submit.  In 
 * other words if we submit at the row level, then we don't need to rename, only if 
 * we submit at the form level in which case all the rows are submitted at once.
 */
(function ($) {
    $.fn.dataPanel = function (options) {
        var defaults = {
            rowLevelSumbit: false,
            mvcController: "",
            mvcEditAction: "",
            mvcDetailsAction: "",
            mvcCreateAction: "",
            mvcDeleteAction: "",
            queryString: "",

            //Above or below the insert point
            insertLocationAbove: true,

            //Callbacks
            onBeforeUpdateCreateDelete: function (action) { return (true); },
            onAfterInsert: function (insertDome) { }
        };
        var _options = $.extend(defaults, options);
        var _container = this;

        var init = function () {
            //Validate state
            if ($(_container).length !== 1) {
                throw ('Only one dome binding at a time is supported currently, requested: ' + $(_container).length);
            }

            //Auto number on start up
            var rowNumber = 0;
            var namePrefix = $(_container).attr("data-panel-prefix");
            $('[data-panel-row]', _container).each(function () {
                if ($(this).closest('[data-panel-container]')[0] === _container[0]) {
                    $(this).attr('data-panel-row-number', rowNumber);
                    rowNumber = rowNumber + 1;
                }
            });

            $(_container).attr('data-panel-container', 'true');
            $(_container).attr('data-panel-row-total', rowNumber);

            //Setup handler
            $("[data-panel-click]", _container).live('click', function () {
                //Don't respond to nested container's clicks
                if ($(this).closest('[data-panel-container]')[0] === _container[0]) {
                    var action = $(this).attr('data-panel-click');
                    var carryOn = true;
                    if (action==="update" || action==="create" || action==="post" || action==="delete") {
                        carryOn = _options.onBeforeUpdateCreateDelete(action);
                    }
                    if (carryOn) {
                        _handleAction(this, $(this).attr('data-panel-click'), false);
                    }
                    return (false);
                }
            });

            //Setup remote handlers for this container (only supports inserts)(
            $("[data-panel-remote-click-target='" + $(_container).attr('id') + "']").live('click', function () {
                _handleAction(this, "insert", true);
                return (false);
            });

            //Debug
            //                        $("[data-panel-row]", _container).live('click', function () {
            //                            var str = 'Model ID: ' + $(this).attr('data-panel-row-id') + "<br />" +
            //                                                  'Row Number: ' + $(this).attr('data-panel-row-number') + "<br />" +
            //                                                  'Container Prefix: ' + $(this).closest('[data-panel-container]').attr('data-panel-prefix') + "<br />" +
            //                                                  'Container Total: ' + $(this).closest('[data-panel-container]').attr('data-panel-row-total') + "<br />";
            //                            var row = this;
            //                            $('[name]', this).each(function () {
            //                                if ($(this).closest('[data-panel-row]')[0] === row) {
            //                                    str = str + "Name: " + $(this).attr('name') + " ";
            //                                    str = str + "Value: " + $(this).val() + "<br />";
            //                                }
            //                            });
            //                            $.jGrowl(str, { life: 10000 });
            //                            //alert(str);
            //                        });

            return (_container);
        }

        var _getUrl = function (row, action) {
            //Row id
            var id="";
            if (row != null && row != 'undefined') id = $(row).attr('data-panel-row-id');

            //Query string
            var qs = _options.queryString;
            if ($.isFunction(_options.queryString))
                qs = _options.queryString(_container);

            //URL with id except insert
            if (_options.mvcController === "") { return (""); }
            if (action === 'insert' || action === 'create') {
                if (_options.mvcCreateAction === "") { return (""); }
                return ('/' + _options.mvcController + '/' + _options.mvcCreateAction + '?' + qs);
            }
            if (action === 'delete') {
                if (_options.mvcDeleteAction === "") { return (""); }
                return ('/' + _options.mvcController + '/' + _options.mvcDeleteAction + '/' + id + '?' + qs);
            }
            if (action === 'update' || action === 'edit') {
                if (_options.mvcEditAction === "") { return (""); }
                return ('/' + _options.mvcController + '/' + _options.mvcEditAction + '/' + id + '?' + qs);
            }
            if (_options.mvcDetailsAction === "") { return (""); }
            return ('/' + _options.mvcController + '/' + _options.mvcDetailsAction + '/' + id + '?' + qs);
        }

        var _handleAction = function (dome, action, isRemote) {
            var namePrefix = $(_container).attr("data-panel-prefix");
            var rowTotal = $(_container).attr("data-panel-row-total") / 1;

            //Insert -------------------------------------------------
            if (action == "insert") {
                var insPoint, insContainer;

                //Find an insert point if we can
                if (isRemote) {
                    insPoint = $("[data-panel-insertpoint=true]:first", _container);
                }
                else {
                    insPoint = $(dome).closest("[data-panel-insertpoint=true]");
                }

                //Find an insert container
                insContainer = _container;
                //For a table, try the body first
                if ($(insContainer).is('table')) {
                    insContainer = $('tbody', _container);
                }

                //The success method
                var success = function (data) {
                    //Is it an error
                    if (data.indexOf("Error:") === 0) {
                        $.jGrowl(data.slice("Error:".length+1), { sticky: true, header: 'Error' });
                        //alert(data);
                        return;                        
                    }

                    //Process incoming data
                    var insertDome = _setupIncoming($(data), namePrefix, rowTotal);

                    //Insert it
                    if (insPoint.length !== 0) {
                        if (_options.insertLocationAbove) {
                            insertDome.insertBefore(insPoint);
                        } else {
                            insertDome.insertAfter(insPoint);
                        }
                    } else {
                        if (_options.insertLocationAbove) {
                            insContainer.prepend(insertDome);
                        } else {
                            insContainer.append(insertDome);
                        }
                    }

                    //Add to the count
                    $(_container).attr('data-panel-row-total', rowTotal + 1);

                    //Call user callback 
                    if (_options.onAfterInsert)
                        _options.onAfterInsert(insertDome);
                };

                //Ajax the insert html
                if (_getUrl(row, action) !== "") {
                    //$('body').css('cursor', 'progress');
                    $('body').addClass('wait');
                    $.ajax({
                        url: _getUrl(null, action),
                        cache: false,
                        success: success,
                        complete: function () {
                            //$('body').css('cursor', 'default');
                            $('body').removeClass('wait');
                        }
                    });
                } else {
                    throw 'Ajax url for the insert template must be specified.';
                }
            }
            //Non insert -------------------------------------------------------
            else {
                var row = $(dome).closest("[data-panel-row]");
                var rowNumber = $(row).attr("[data-panel-row-number]") / 1;

                if (action === 'create-cancel') {
                    $(row).remove();
                    _renumberContainer(_container, namePrefix);
                }
                if (action === 'update-cancel' || action === 'edit' || action === 'refresh') {
                    if (_getUrl(row, action) !== "") {
                        //$('body').css('cursor', 'progress');
                        $('body').addClass('wait');
                        $.ajax({
                            url: _getUrl(row, action),
                            cache: false,
                            success: function (data) {
                                $(row).replaceWith(_setupIncoming($(data), namePrefix, rowNumber));
                            },
                            complete: function () {
                                //$('body').css('cursor', 'default');
                                $('body').removeClass('wait');
                            }
                        });
                    } else {
                        throw 'Update-cancel/edit/refresh action does not have a url specified.';
                    }
                }
                if (action === 'delete') {
                    var success = function (data) {
                        //The row was deleted.
                        if (data === "Success") {
                            //$(row).slideUp('slow', function () {
                            //  $(row).remove();
                            //});
                            $(row).remove();
                            _renumberContainer(_container, namePrefix);
                        }
                        //The row was not deleted, so we have a message as to why
                        else if (data !== null) {
                            $.jGrowl(data, { sticky: true, header: 'Error', theme: 'jgrowl-error-notification' });
                            //alert(data);
                        }
                    }
                    if (_getUrl(row, action) !== "") {
                        //$('body').css('cursor', 'progress');
                        $('body').addClass('wait');
                        $.ajax({
                            url: _getUrl(row, action),
                            cache: false,
                            success: success,
                            complete: function () {
                                //$('body').css('cursor', 'default');
                                $('body').removeClass('wait');
                            }
                        });
                    } else {
                        //This always succeeds
                        success("Success");
                    }
                }
                if (action === 'update' || action === 'create' || action === 'post') {
                    if (_getUrl(row, action) !== "") {
                        //$('body').css('cursor', 'progress');
                        $('body').addClass('wait');
                        $.ajax({
                            url: _getUrl(row, action),
                            cache: false,
                            type: 'post',
                            data: $(':input', row).serialize(),
                            success: function (data) {
                                $(row).replaceWith(_setupIncoming($(data), namePrefix, rowNumber));
                            },
                            complete: function () {
                                //$('body').css('cursor', 'default');
                                $('body').removeClass('wait');
                            }
                        });
                    } else {
                        throw 'Update/create/post action url not specified.';
                    }
                }
            }
        };

        var _setupIncoming = function (dome, namePrefix, rowNumber) {
            //Prefix incoming names
            if (namePrefix !== "") {
                $('[name]', dome).each(function () {
                    $(this).attr('name', namePrefix + '[' + rowNumber + '].' + $(this).attr('name'));
                });
            }

            //Find the row
            var row = dome;
            if (!$(dome).is('[data-panel-row]')) {
                row = $('first:[data-panel-row]', dome);
            }
            if ($(row).length === 0) {
                throw 'Unable to find data-panel-row in incoming row';
            }
            if (($(row).attr('data-panel-row-id') || "") === "") {
                throw ('data-panel-row return without data-panel-row-id attribute');
            }

            $(row, dome).attr('data-panel-row-number', rowNumber);
            return (dome);
        };

        var _renumberContainer = function (container, namePrefix) {
            var index = 0;

            //Loop through each row in this container
            $('[data-panel-row]', container).each(function () {
                if ($(this).closest('[data-panel-container]')[0] === container[0]) {
                    var row = this;
                    $(this).attr('data-panel-row-number', index);

                    //If we need to rename the fields for a full form submit.  In other words if we
                    //submit at the row level, then we don't need to rename, only if we submit at
                    //the form level in which case all the rows are submitted at once.
                    if (!_options.rowLevelSubmit) {

                        //Loop through each named tag in this row, including sub-containers
                        $('[name],[data-panel-prefix]', row).each(function () {
                            //Replace name
                            val = $(this).attr("name") || "";
                            if (val !== "" && (namePrefix === "" || val.indexOf(namePrefix) === 0)) {
                                var pclose = val.indexOf(']', namePrefix.length);
                                var nval = namePrefix + '[' + index + ']' + val.slice(pclose + 1);
                                if (val != nval) {
                                    $(this).attr("name", nval);
                                }
                            }

                            //Replace namePrefix
                            val = $(this).attr("data-panel-prefix") || "";
                            if (val !== "" && (namePrefix === "" || val.indexOf(namePrefix) === 0)) {
                                var pclose = val.indexOf(']', namePrefix.length);
                                var nval = namePrefix + '[' + index + ']' + val.slice(pclose + 1);
                                if (val != nval) {
                                    $(this).attr("data-panel-prefix", nval);
                                }
                            }
                        });
                    }

                    index = index + 1;
                }
            });

            $(_container).attr('data-panel-row-total', index);
        }
        return (init());
    }
})(jQuery); 
