function siteFunction() {
    //form submit wire up with button class to prevent for multiple clicks
    $('form').submit(function () {
        return false;
    });
    $(".btnUpdate, .btnCreate").click(function () {
        this.disabled = true;
        this.value = 'Submitting...';
        this.form.submit();
    });
    //Configure ajax defaults to no-cache
    $(function () {
        $.ajaxSetup({ cache: false });
    });
    $(function () {
        $('.btnDelete').click(function () {
            return (confirm('Are you sure you wish to delete this data?'));
        });
    });
}