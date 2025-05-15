document.addEventListener('DOMContentLoaded', function () {
    const categorySelect = document.querySelector('select[name="Category"]') || document.getElementById('Category');
    const subcategorySelect = document.querySelector('select[name="SubCategory"]') || document.getElementById('SubCategory');
    const meta = document.querySelector('meta[name="api-base-url"]');
    const API_BASE_URL = meta ? meta.content : '';

    if (!categorySelect || !subcategorySelect) {
        return;
    }

    // Create error message element
    let errorMessageDiv = document.createElement('div');
    errorMessageDiv.className = 'alert alert-danger';
    errorMessageDiv.style.display = 'none';
    errorMessageDiv.id = 'errorMessage';

    // Create loading indicator with improved styling
    let loadingIndicator = document.createElement('div');
    loadingIndicator.className = 'loading-indicator';
    loadingIndicator.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Loading subcategories...';
    loadingIndicator.style.display = 'none';
    loadingIndicator.id = 'loadingSubcategories';

    // Insert elements after the subcategory select
    subcategorySelect.parentNode.insertBefore(errorMessageDiv, subcategorySelect.nextSibling);
    subcategorySelect.parentNode.insertBefore(loadingIndicator, subcategorySelect.nextSibling);

    function loadSubcategories(category) {
        if (!category) {
            subcategorySelect.innerHTML = '<option value="">-- Select Subcategory --</option>';
            return;
        }

        loadingIndicator.style.display = 'block';
        errorMessageDiv.style.display = 'none';

        fetch(`${API_BASE_URL}api/subcategories/${encodeURIComponent(category)}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Failed to load subcategories: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                subcategorySelect.innerHTML = '<option value="">-- Select Subcategory --</option>';
                const original = subcategorySelect.dataset.selected || "";

                if (Array.isArray(data)) {
                    data.forEach(item => {
                        const option = document.createElement('option');
                        const value = typeof item === 'object' ? (item.subCategory || item.SubCategory) : item;
                        option.value = value;
                        option.textContent = value;

                        if (value === original) {
                            option.selected = true;
                        }

                        subcategorySelect.appendChild(option);
                    });

                    // Add animation to show new options
                    subcategorySelect.classList.add('options-loaded');
                    setTimeout(() => {
                        subcategorySelect.classList.remove('options-loaded');
                    }, 500);
                }

                loadingIndicator.style.display = 'none';
            })
            .catch(error => {
                console.error('Error loading subcategories:', error);
                errorMessageDiv.innerHTML = '<i class="fas fa-exclamation-circle me-2"></i>Failed to load subcategories. Please try again.';
                errorMessageDiv.style.display = 'block';
                loadingIndicator.style.display = 'none';
            });
    }

    categorySelect.addEventListener('change', function () {
        loadSubcategories(this.value);
    });

    // Load subcategories on page load if category is already selected
    if (categorySelect.value) {
        loadSubcategories(categorySelect.value);
    }

    // Add validation for subcategory when submitting the form
    const form = categorySelect.closest('form');
    if (form) {
        form.addEventListener('submit', function (event) {
            if (categorySelect.value && !subcategorySelect.value) {
                event.preventDefault();
                errorMessageDiv.innerHTML = '<i class="fas fa-exclamation-circle me-2"></i>Please select a subcategory';
                errorMessageDiv.style.display = 'block';

                // Scroll to error message
                errorMessageDiv.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        });
    }
});