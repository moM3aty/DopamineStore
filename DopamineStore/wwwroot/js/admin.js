function initializeProductEditPage(productId) {
    const galleryContainer = document.getElementById('image-gallery-container');
    const imageUploadForm = document.getElementById('image-upload-form');
    const deleteModalElement = document.getElementById('deleteImageModal');
    const deleteModal = new bootstrap.Modal(deleteModalElement);
    const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');
    let imageIdToDelete = null;

    galleryContainer.addEventListener('click', function (e) {
        const deleteBtn = e.target.closest('.delete-image-btn');
        const setMainBtn = e.target.closest('.set-main-image-btn');

        if (deleteBtn) {
            imageIdToDelete = deleteBtn.dataset.imageId;
            deleteModal.show();
        }

        if (setMainBtn) {
            const imageId = setMainBtn.dataset.imageId;
            setMainImage(productId, imageId);
        }
    });

    confirmDeleteBtn.addEventListener('click', function () {
        if (imageIdToDelete) {
            deleteImage(imageIdToDelete);
        }
    });

    imageUploadForm.addEventListener('submit', function (e) {
        e.preventDefault();
        const formData = new FormData(this);
        const button = this.querySelector('button[type="submit"]');
        const originalButtonText = button.innerHTML;
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري الرفع...';

        fetch(`/Admin/Products/AddImages/${productId}`, {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        })
            .then(response => response.text())
            .then(html => {
                galleryContainer.innerHTML = html;
                imageUploadForm.reset();
            })
            .catch(error => console.error('Error uploading images:', error))
            .finally(() => {
                button.disabled = false;
                button.innerHTML = originalButtonText;
            });
    });

    function deleteImage(imageId) {
        fetch(`/Admin/Products/DeleteImage`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({ imageId: imageId })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    document.getElementById(`image-container-${imageId}`).remove();
                    window.location.hash = "#images-pane";
                    window.location.reload();
                } else {
                    alert(data.message || 'فشل حذف الصورة');
                }
            })
            .catch(error => console.error('Error deleting image:', error))
            .finally(() => {
                deleteModal.hide();
                imageIdToDelete = null;
            });
    }

    function setMainImage(productId, imageId) {
        fetch(`/Admin/Products/SetMainImage`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({ productId: productId, imageId: imageId })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    window.location.hash = "#images-pane";
                    window.location.reload();
                } else {
                    alert(data.message || 'فشل تحديد الصورة الرئيسية');
                }
            })
            .catch(error => console.error('Error setting main image:', error));
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const hash = window.location.hash;
    if (hash) {
        const tabTrigger = document.querySelector(`a[href="${hash}"]`);
        if (tabTrigger) {
            new bootstrap.Tab(tabTrigger).show();
        }
    }

    const addCategoryModalElement = document.getElementById('addCategoryModal');
    if (addCategoryModalElement) {
        const saveCategoryBtn = document.getElementById('saveCategoryBtn');
        const newCategoryNameInput = document.getElementById('newCategoryName');
        const errorAlert = document.getElementById('modal-error-alert');
        const spinner = document.getElementById('save-spinner');
        const categoryDropdown = document.getElementById('category-dropdown');
        const addCategoryModal = new bootstrap.Modal(addCategoryModalElement);

        saveCategoryBtn.addEventListener('click', function () {
            const categoryName = newCategoryNameInput.value.trim();
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            if (!categoryName) return;

            this.disabled = true;
            spinner.style.display = 'inline-block';
            errorAlert.style.display = 'none';

            fetch('/Admin/Categories/CreateFromModal', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ name: categoryName })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        const newOption = new Option(data.name, data.id, true, true);
                        categoryDropdown.appendChild(newOption);
                        categoryDropdown.dispatchEvent(new Event('change'));
                        addCategoryModal.hide();
                        newCategoryNameInput.value = '';
                    } else {
                        errorAlert.textContent = data.message;
                        errorAlert.style.display = 'block';
                    }
                })
                .catch(error => {
                    errorAlert.textContent = 'حدث خطأ غير متوقع.';
                    errorAlert.style.display = 'block';
                    console.error('Error creating category:', error);
                })
                .finally(() => {
                    this.disabled = false;
                    spinner.style.display = 'none';
                });
        });
    }
});
