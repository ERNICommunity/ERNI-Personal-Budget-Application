window.foo = {
    bar: function() {
        //alert('Bar called');
        DotNet.invokeMethodAsync("ERNI.PBA.Client", "CallFromJs")
    },
    alert: function () {
        alert('Blazor called js to call blazor to call js');
    }
}