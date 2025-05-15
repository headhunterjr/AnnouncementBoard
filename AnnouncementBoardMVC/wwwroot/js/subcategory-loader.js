document.addEventListener('DOMContentLoaded', function () {
    const categorySelect = document.querySelector('select[name="Category"]') || document.getElementById('Category');
    const subcategorySelect = document.querySelector('select[name="SubCategory"]') || document.getElementById('SubCategory');

    const API_BASE_URL = 'https://localhost:7111';

    if (!categorySelect || !subcategorySelect) {
        return;
    }

    let errorMessageDiv = document.createElement('div');
    errorMessageDiv.className = 'alert alert-danger';
    errorMessageDiv.style.display = 'none';
    errorMessageDiv.id = 'errorMessage';
    categorySelect.parentNode.insertBefore(errorMessageDiv, categorySelect.nextSibling);

    let loadingIndicator = document.createElement('span');
    loadingIndicator.textContent = ' Loading...';
    loadingIndicator.style.display = 'none';
    loadingIndicator.id = 'loadingSubcategories';
    subcategorySelect.parentNode.insertBefore(loadingIndicator, subcategorySelect.nextSibling);

    function loadSubcategories(category) {
        if (!category) {
            subcategorySelect.innerHTML = '<option value="">-- choose --</option>';
            return;
        }

        loadingIndicator.style.display = 'inline';

        fetch(`${API_BASE_URL}/api/subcategories/${encodeURIComponent(category)}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Failed to load subcategories: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                subcategorySelect.innerHTML = '<option value="">-- choose --</option>';

                if (Array.isArray(data)) {
                    data.forEach(item => {
                        const option = document.createElement('option');
                        const value = typeof item === 'object' ? (item.subCategory || item.SubCategory) : item;
                        option.value = value;
                        option.textContent = value;
                        subcategorySelect.appendChild(option);
                        console.log('Added option:', value);
                    });
                }

                loadingIndicator.style.display = 'none';

                errorMessageDiv.style.display = 'none';
            })
            .catch(error => {
                errorMessageDiv.textContent = 'Failed to load subcategories. Please try again.';
                errorMessageDiv.style.display = 'block';
                loadingIndicator.style.display = 'none';
            });
    }

    categorySelect.addEventListener('change', function () {
        loadSubcategories(this.value);
    });

    if (categorySelect.value) {
        loadSubcategories(categorySelect.value);
    }

    const form = categorySelect.closest('form');
    if (form) {
        form.addEventListener('submit', function (event) {
            if (categorySelect.value && !subcategorySelect.value) {
                event.preventDefault();
                errorMessageDiv.textContent = 'Please select a subcategory';
                errorMessageDiv.style.display = 'block';
            }
        });
    }
});