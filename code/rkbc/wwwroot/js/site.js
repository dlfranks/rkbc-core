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
    //ajax-loader
    function hideLoader() {
        $('#loading').hide();
    }
    
    $(window).ready(hideLoader);

    // Strongly recommended: Hide loader after 20 seconds, even if the page hasn't finished loading
    setTimeout(hideLoader, 20 * 1000);
}
$('#loading').show();