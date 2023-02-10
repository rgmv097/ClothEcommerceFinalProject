
$(document).on('click', '#add-product-to-cart', function () {
    var productId = $(this).parent().children().val();

    var productSize = $('#product-size').find(":selected").val();
    var productColor = $('#product-color').find(":selected").val();
    console.log(productSize)
    $.ajax({
        type: "POST",
        url: '/Basket/AddToBasket',
        data: { productId: productId, productSize: productSize, productColor: productColor },
        success: function () {
        },
    });

})

$(document).on('click', '#remove-from-cart', function () {
    var productId = $(this).parent().parent().children().children().val();
    console.log(productId)

    $.ajax({
        type: "POST",
        url: "/Basket/DeleteProductBasket",
        data: { productId: productId },
        success: function () {
            $("#" + productId + "tbody").remove();
        },
        Error: function () {
            alert("Somthing Wrong");
        }
    });
});

$(document).on('change', '.product-qty input', function () {
    const parent = $(this).parents("tr");
    var productId = parent.find("[data-product-id]").val();
    var productPrice = parent.find(".base-price > span").data('price');
    var productQuantity = $(this).val();

    var totalPrice = productQuantity * parseFloat(productPrice);

    $.ajax({
        type: "post",
        url: "/basket/ChangeProductQuality",
        data: { productId: productId, count: productQuantity },
        success: function () {
            parent.find(".total-price > span").text(`₼${totalPrice}`)
        },
        error: function () {
            alert("somthing wrong");
        }
    });


});
