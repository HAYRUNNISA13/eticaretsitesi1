// Modern E-ticaret Javascript Fonksiyonları
document.addEventListener('DOMContentLoaded', function() {
    // Back to top düğmesi fonksiyonu
    var backToTopBtn = document.querySelector('.back-to-top');
    
    if (backToTopBtn) {
        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 300) {
                backToTopBtn.style.display = 'block';
            } else {
                backToTopBtn.style.display = 'none';
            }
        });
        
        backToTopBtn.addEventListener('click', function(e) {
            e.preventDefault();
            window.scrollTo({top: 0, behavior: 'smooth'});
        });
    }
    
    // Ürün kartlarına hover efekti
    var productCards = document.querySelectorAll('.product-card');
    productCards.forEach(function(card) {
        card.classList.add('shadow-sm');
        
        // Kart içi elemanların doğru hizalanması
        var cardBody = card.querySelector('.card-body');
        if (cardBody) {
            cardBody.style.display = 'flex';
            cardBody.style.flexDirection = 'column';
            
            var cardText = cardBody.querySelector('.card-text');
            if (cardText) {
                cardText.style.flexGrow = '1';
            }
        }
    });
    
    // Animasyonlar için fade-in sınıfı ekle
    var fadeElements = document.querySelectorAll('.card, .section-title, .product-detail-card');
    fadeElements.forEach(function(element, index) {
        // Kademeli olarak animasyon ekle
        setTimeout(function() {
            element.classList.add('fade-in');
        }, index * 50); // Her elemana 50ms gecikmeli animasyon
    });
    
    // Dropdown menüler için hover etkinleştirme (desktop)
    var dropdowns = document.querySelectorAll('.dropdown');
    if (window.innerWidth >= 992) {
        dropdowns.forEach(function(dropdown) {
            dropdown.addEventListener('mouseover', function() {
                this.querySelector('.dropdown-menu').classList.add('show');
            });
            dropdown.addEventListener('mouseout', function() {
                this.querySelector('.dropdown-menu').classList.remove('show');
            });
        });
    }
    
    // Navbar scroll olduğunda davranış değişikliği
    var navbar = document.querySelector('.navbar');
    if (navbar) {
        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 100) {
                navbar.classList.add('navbar-scrolled');
            } else {
                navbar.classList.remove('navbar-scrolled');
            }
        });
    }
    
    // Ürün görselleri için hover zoom efekti
    var productImages = document.querySelectorAll('.product-card .card-img-top');
    productImages.forEach(function(img) {
        img.parentElement.classList.add('overflow-hidden');
    });
    
    // Ürün kartlarındaki metinlerin eşit yükseklikte olması
    equalizeCardTextHeights();
    
    // Pencere yeniden boyutlandığında kartların yüksekliğini eşitle
    window.addEventListener('resize', debounce(function() {
        equalizeCardTextHeights();
    }, 250));
    
    // Sepete ekleme butonları için animasyon
    var addToCartButtons = document.querySelectorAll('.add-to-cart');
    addToCartButtons.forEach(function(button) {
        button.addEventListener('click', function(e) {
            this.classList.add('btn-adding');
            var btn = this;
            setTimeout(function() {
                btn.classList.remove('btn-adding');
            }, 1000);
        });
    });
});

// Kart metinlerinin yüksekliğini eşitleme
function equalizeCardTextHeights() {
    // Her satırdaki kartlar için
    document.querySelectorAll('.row').forEach(function(row) {
        var cardTitles = row.querySelectorAll('.card-title');
        var cardTexts = row.querySelectorAll('.card-text');
        
        // Başlıkların yüksekliğini sıfırla
        cardTitles.forEach(function(title) {
            title.style.height = 'auto';
        });
        
        // Metinlerin yüksekliğini sıfırla
        cardTexts.forEach(function(text) {
            text.style.height = 'auto';
        });
        
        // Eğer masaüstü görünümünde ise yükseklikleri eşitle
        if (window.innerWidth >= 768) {
            // En yüksek başlık yüksekliğini bul
            var maxTitleHeight = 0;
            cardTitles.forEach(function(title) {
                maxTitleHeight = Math.max(maxTitleHeight, title.offsetHeight);
            });
            
            // En yüksek metin yüksekliğini bul
            var maxTextHeight = 0;
            cardTexts.forEach(function(text) {
                maxTextHeight = Math.max(maxTextHeight, text.offsetHeight);
            });
            
            // Tüm başlıklara aynı yüksekliği ver
            cardTitles.forEach(function(title) {
                title.style.height = maxTitleHeight + 'px';
            });
            
            // Tüm metinlere aynı yüksekliği ver
            cardTexts.forEach(function(text) {
                text.style.height = maxTextHeight + 'px';
            });
        }
    });
}

// Performans için debounce fonksiyonu
function debounce(func, wait) {
    var timeout;
    return function() {
        var context = this, args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(function() {
            func.apply(context, args);
        }, wait);
    };
}

// Hızlı bakış fonksiyonları
function showQuickView(productId) {
    var modal = $('#quickViewModal');
    var content = $('#quickViewContent');
    
    // Modali göster ve loading durumuna getir
    modal.modal('show');
    content.html('<div class="d-flex justify-content-center"><div class="loader"></div></div>');
    
    // Ajax ile ürün bilgilerini getir
    $.get('/Product/QuickView/' + productId, function(data) {
        content.html(data);
    });
}

// Bildirim gösterme fonksiyonu
function showNotification(title, message, type = 'success') {
    var toast = $('#notificationToast');
    $('#toastTitle').text(title);
    $('#toastMessage').text(message);
    
    // Toast sınıfını temizle ve yeni tip ekle
    toast.removeClass('bg-success bg-danger bg-warning bg-info');
    
    switch(type) {
        case 'success':
            toast.addClass('bg-success text-white');
            break;
        case 'error':
            toast.addClass('bg-danger text-white');
            break;
        case 'warning':
            toast.addClass('bg-warning');
            break;
        case 'info':
            toast.addClass('bg-info');
            break;
    }
    
    // Bootstrap toast'ı göster
    var toastInstance = new bootstrap.Toast(toast);
    toastInstance.show();
}
