(function () {
    "use strict";

    const toEasternArabicNumerals = (str) => {
        if (str === null || str === undefined) return '';
        let numStr = String(str);
        const arabicNumerals = ['٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩'];
        return numStr.replace(/[0-9]/g, w => arabicNumerals[+w]);
    };

  
    const select = (el, all = false) => {
        el = el.trim()
        if (all) {
            return [...document.querySelectorAll(el)]
        } else {
            return document.querySelector(el)
        }
    }

    const on = (type, el, listener, all = false) => {
        let selectEl = select(el, all)
        if (selectEl) {
            if (all) {
                selectEl.forEach(e => e.addEventListener(type, listener))
            } else {
                selectEl.addEventListener(type, listener)
            }
        }
    }


    const onscroll = (el, listener) => {
        el.addEventListener('scroll', listener)
    }


    const scrollto = (el) => {
        let header = select('#header')
        let offset = header ? header.offsetHeight : 0;

        let elementPos = select(el).offsetTop
        window.scrollTo({
            top: elementPos - offset,
            behavior: 'smooth'
        })
    }


    let preloader = select('#preloader');
    if (preloader) {
        window.addEventListener('load', () => {
            preloader.remove()
        });
    }

  
    let backtotop = select('.back-to-top')
    if (backtotop) {
        const toggleBacktotop = () => {
            if (window.scrollY > 100) {
                backtotop.classList.add('active')
            } else {
                backtotop.classList.remove('active')
            }
        }
        window.addEventListener('load', toggleBacktotop)
        onscroll(document, toggleBacktotop)
        on('click', '.back-to-top', function (e) {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    on('click', '.mobile-nav-toggle', function (e) {
        select('body').classList.toggle('mobile-nav-active')
        this.classList.toggle('bi-list')
        this.classList.toggle('bi-x')
    })


    on('click', '.scrollto', function (e) {
        if (select(this.hash)) {
            e.preventDefault()

            let body = select('body')
            if (body.classList.contains('mobile-nav-active')) {
                body.classList.remove('mobile-nav-active')
                let navbarToggle = select('.mobile-nav-toggle')
                navbarToggle.classList.toggle('bi-list')
                navbarToggle.classList.toggle('bi-x')
            }
            scrollto(this.hash)
        }
    }, true)

    document.addEventListener("DOMContentLoaded", function () {
        const filters = document.querySelectorAll("#portfolio-flters li");
        const products = document.querySelectorAll(".portfolio-item");

        function showLastSix(categoryId) {
            products.forEach(p => {
                p.classList.remove("show");
                setTimeout(() => {
                    p.style.display = "none";
                }, 300);
            });

            setTimeout(() => {
                const matching = Array.from(products).filter(p => p.dataset.category === categoryId);
                const selected = matching.slice(-6);
                selected.forEach(p => {
                    p.style.display = "block";
                    setTimeout(() => p.classList.add("show"), 10);
                });
            }, 350);
        }

        if (filters.length > 0) {
            const firstFilter = filters[0];
            firstFilter.classList.add("filter-active");
            showLastSix(firstFilter.dataset.filter);
        }

        filters.forEach(f => {
            f.addEventListener("click", function () {
                filters.forEach(el => el.classList.remove("filter-active"));
                this.classList.add("filter-active");
                showLastSix(this.dataset.filter);
            });
        });
    });

 
    const initializeCountdown = () => {
        const countdowns = document.querySelectorAll('.countdown-timer, .countdown-timer-details');
        countdowns.forEach(countdown => {
            const endDate = new Date(countdown.dataset.endDate).getTime();
            const interval = setInterval(() => {
                const now = new Date().getTime();
                const distance = endDate - now;

                if (distance < 0) {
                    countdown.innerHTML = "<div class='time-block' style='width:100%;'><span>انتهى العرض</span></div>";
                    clearInterval(interval);
                    return;
                }

                const days = toEasternArabicNumerals(Math.floor(distance / (1000 * 60 * 60 * 24)));
                const hours = toEasternArabicNumerals(Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)));
                const minutes = toEasternArabicNumerals(Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60)));
                const seconds = toEasternArabicNumerals(Math.floor((distance % (1000 * 60)) / 1000));

                countdown.innerHTML = `
                    <div class="time-block"><span class="time-value">${days}</span><span class="time-label">أيام</span></div>
                    <div class="time-block"><span class="time-value">${hours}</span><span class="time-label">ساعات</span></div>
                    <div class="time-block"><span class="time-value">${minutes}</span><span class="time-label">دقائق</span></div>
                    <div class="time-block"><span class="time-value">${seconds}</span><span class="time-label">ثواني</span></div>
                `;
            }, 1000);
        });
    };
    window.addEventListener('load', initializeCountdown);

 
    on('click', '.thumbnail-item', function () {
        select('.thumbnail-item', true).forEach(item => item.classList.remove('active'));
        this.classList.add('active');
        document.getElementById('main-product-image').src = this.querySelector('img').src;
    }, true);

})();

$(document).ready(function () {

    const toEasternArabicNumerals = (str) => {
        if (str === null || str === undefined) return '';
        let numStr = String(str);
        const arabicNumerals = ['٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩'];
        return numStr.replace(/[0-9]/g, w => arabicNumerals[+w]);
    };

    function showToast(message, type = 'success') {
        $('.toast-notification').remove();
        var toast = $('<div class="toast-notification"></div>');
        toast.text(message);
        toast.addClass(type);
        $('body').append(toast);
        setTimeout(function () {
            toast.addClass('show');
        }, 100);
        setTimeout(function () {
            toast.removeClass('show');
            setTimeout(function () {
                toast.remove();
            }, 500);
        }, 3000);
    }

    function convertAllNumbersToArabic() {
        $('.current-price, .old-price, .sale-label, .featured-discount-badge, .offer-current-price, .offer-old-price, #cart-badge, .numeric-value').each(function () {
            var element = $(this);
            function traverse(node) {
                if (node.nodeType === 3) { 
                    var text = node.nodeValue;
                    var convertedText = toEasternArabicNumerals(text);
                    if (text !== convertedText) {
                        node.nodeValue = convertedText;
                    }
                } else if (node.nodeType === 1 && node.childNodes && node.nodeName.toLowerCase() !== 'script') { 
                    for (var i = 0; i < node.childNodes.length; i++) {
                        traverse(node.childNodes[i]);
                    }
                }
            }
            traverse(element[0]);
        });
    }

    function updateCart() {
        $.get('/Cart/GetCartState', function (response) {
            if (response.success) {
                $('#slide-cart-body').html(response.html);
                $('#cart-badge').text(toEasternArabicNumerals(response.cartCount));
                convertAllNumbersToArabic();
                const badge = $('#cart-badge');
                badge.addClass('updated');
                setTimeout(() => badge.removeClass('updated'), 300);
            }
        });
    }

    updateCart();
    convertAllNumbersToArabic();

    if (typeof Isotope !== 'undefined' && $('.portfolio-container').length) {
        var iso = Isotope.data($('.portfolio-container')[0]);
        if (iso) {
            iso.on('arrangeComplete', function () {
                convertAllNumbersToArabic();
            });
        }
    }

    $(document).on('click', '.btn-add-to-cart[data-product-id]', function (e) {
        if (!$(this).closest('form').length) {
            e.preventDefault();

            var button = $(this);
            var productCard = button.closest('.product-card-v2');
            var productName = productCard.find('.product-title a').text();
            var productImageSrc = productCard.find('.product-image img').attr('src');
            var productId = button.data('product-id');

            $('#modalProductName').text(productName);
            $('#modalProductImage').attr('src', productImageSrc);
            $('#modalProductIdInput').val(productId);
            $('#modalQuantityInput').val(1);

            var quantityModal = new bootstrap.Modal(document.getElementById('quantityModal'));
            quantityModal.show();
        }
    });

    $('#addToCartModalForm').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        $.ajax({
            type: 'POST',
            url: form.attr('action'),
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    updateCart();
                    var quantityModal = bootstrap.Modal.getInstance(document.getElementById('quantityModal'));
                    quantityModal.hide();
                    var slideCart = new bootstrap.Offcanvas(document.getElementById('slideCart'));
                    slideCart.show();
                } else {
                    showToast(response.message || 'حدث خطأ ما', 'error');
                }
            },
            error: function () {
                showToast('حدث خطأ في الاتصال بالخادم.', 'error');
            }
        });
    });

    $('.add-to-cart-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var quantityInput = form.find('.quantity-input');
        var quantity = parseInt(quantityInput.val());
        var maxStock = parseInt(quantityInput.attr('max'));

        if (maxStock <= 0) {
            showToast('عذراً, هذا المنتج غير متوفر حالياً.', 'error');
            return;
        }
        if (quantity > maxStock) {
            showToast('الكمية المطلوبة أكبر من المخزون المتاح!', 'error');
            return;
        }

        $.ajax({
            type: 'POST',
            url: form.attr('action'),
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    updateCart();
                    showToast('تمت إضافة المنتج إلى السلة بنجاح!', 'success');
                } else {
                    showToast(response.message || 'حدث خطأ ما', 'error');
                }
            },
            error: function () {
                showToast('حدث خطأ في الاتصال بالخادم.', 'error');
            }
        });
    });

    $(document).on('click', '.wishlist-btn, .wishlist-btn-details', function (e) {
        e.preventDefault();
        var button = $(this);
        var productId = button.data('product-id');
        var token = $('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').val();

        $.ajax({
            type: 'POST',
            url: '/Wishlist/AddOrRemove',
            data: {
                __RequestVerificationToken: token,
                productId: productId
            },
            success: function (response) {
                if (response.success) {
                    button.toggleClass('is-wished', response.added);
                    showToast(response.added ? 'تمت الإضافة إلى المفضلة' : 'تمت الإزالة من المفضلة');
                } else {
                    showToast(response.message, 'error');
                    if (response.redirectTo) {
                        setTimeout(function () {
                            window.location.href = response.redirectTo;
                        }, 1500);
                    }
                }
            },
            error: function () {
                showToast('حدث خطأ ما. يرجى المحاولة مرة أخرى.', 'error');
            }
        });
    });

    $('#contact-form-whatsapp').on('submit', function (e) {
        e.preventDefault();
        var name = $('#Name').val();
        var email = $('#Email').val();
        var subject = $('#Subject').val();
        var message = $('#Message').val();
        var phoneNumber = "201202654696";
        var fullMessage = `رسالة جديدة من متجر دوبامين:\n\n*الاسم:* ${name}\n*البريد الإلكتروني:* ${email}\n*الموضوع:* ${subject}\n\n*الرسالة:*\n${message}`;
        var encodedMessage = encodeURIComponent(fullMessage);
        var whatsappUrl = `https://wa.me/${phoneNumber}?text=${encodedMessage}`;
        window.open(whatsappUrl, '_blank');
    });

    $(document).on('click', '.quantity-selector .quantity-plus', function () {
        var input = $(this).siblings('.quantity-input');
        var currentValue = parseInt(input.val());
        var maxAttr = input.attr('max');
        var max = (maxAttr !== undefined && !isNaN(parseInt(maxAttr))) ? parseInt(maxAttr) : Infinity;
        if (currentValue < max) {
            input.val(currentValue + 1);
        }
    });

    $(document).on('click', '.quantity-selector .quantity-minus', function () {
        var input = $(this).siblings('.quantity-input');
        var currentValue = parseInt(input.val());
        if (currentValue > 1) {
            input.val(currentValue - 1);
        }
    });

    if ($('.product-details-section').length) {
        $('.star-rating-input .bi').on('mouseover', function () {
            $(this).addClass('filled').prevAll().addClass('filled');
            $(this).nextAll().removeClass('filled');
        }).on('mouseout', function () {
            var selectedValue = $('#rating-value').val();
            $('.star-rating-input .bi').each(function () {
                if ($(this).data('value') <= selectedValue) {
                    $(this).addClass('filled');
                } else {
                    $(this).removeClass('filled');
                }
            });
        }).on('click', function () {
            var value = $(this).data('value');
            $('#rating-value').val(value);
            $(this).addClass('filled').prevAll().addClass('filled');
            $(this).nextAll().removeClass('filled');
        });
    }
});
