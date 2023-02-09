
$(document).on('click', '#add-product-to-cart', function () {

    var productId = $(this).parent().children().val();
    $.ajax({
        type: "POST",
        url: '/Basket/AddToBasket',
        data: { productId: productId },
        success: function () {
        },
    });

})

$('#product-size').on('change', function () {
    alert(this.value);
});
