document.addEventListener('DOMContentLoaded', function () {
    const categoryCheckboxes = document.querySelectorAll('.category-checkbox');
    const subcategoriesContainer = document.getElementById('subcategoriesContainer');
    const meta = document.querySelector('meta[name="api-base-url"]');
    const API_BASE_URL = meta ? meta.content : '';


    categoryCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const category = this.dataset.category;

            const selectedCategories = [];
            document.querySelectorAll('.category-checkbox:checked').forEach(cb => {
                selectedCategories.push(cb.value);
            });

            if (selectedCategories.length === 0) {
                subcategoriesContainer.innerHTML = '<p class="text-muted p-3">Select categories to see subcategories</p>';
                return;
            }

            const loadingDiv = document.createElement('div');
            loadingDiv.className = 'text-center p-3';
            loadingDiv.innerHTML = '<div class="spinner-border text-primary" role="status">' +
                '<span class="visually-hidden">Loading...</span></div>' +
                '<p class="mt-2">Loading subcategories...</p>';

            let loadedCategories = 0;
            const existingGroups = {};

            selectedCategories.forEach(category => {
                fetch(`${API_BASE_URL}api/subcategories/${encodeURIComponent(category)}`, {
                    method: 'GET',
                    mode: 'cors',
                    headers: {
                        'Accept': 'application/json'
                    }
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error(`Failed to load subcategories for ${category}`);
                        }
                        return response.json();
                    })
                    .then(data => {
                        loadedCategories++;

                        if (loadedCategories === 1) {
                            subcategoriesContainer.innerHTML = '';
                        }

                        const groupDiv = document.createElement('div');
                        groupDiv.className = 'subcategory-group';
                        groupDiv.dataset.parentCategory = category;

                        const heading = document.createElement('h6');
                        heading.className = 'mt-3';
                        heading.textContent = category;
                        groupDiv.appendChild(heading);

                        const checkboxContainer = document.createElement('div');
                        checkboxContainer.className = 'ms-3';

                        if (Array.isArray(data) && data.length > 0) {
                            data.forEach(subcategory => {
                                const value = typeof subcategory === 'object' ?
                                    (subcategory.subCategory || subcategory.SubCategory) :
                                    subcategory;

                                const checkDiv = document.createElement('div');
                                checkDiv.className = 'form-check';

                                checkDiv.innerHTML = `
                                        <input class="form-check-input"
                                               type="checkbox"
                                               value="${value}"
                                               id="subcategory_${value}"
                                               name="SelectedSubCategories"
                                               checked>
                                        <label class="form-check-label" for="subcategory_${value}">
                                            ${value}
                                        </label>
                                    `;

                                checkboxContainer.appendChild(checkDiv);
                            });
                        } else {
                            checkboxContainer.innerHTML = '<p class="text-muted">No subcategories available</p>';
                        }

                        groupDiv.appendChild(checkboxContainer);
                        subcategoriesContainer.appendChild(groupDiv);
                    })
                    .catch(error => {
                        console.error('Error loading subcategories:', error);

                        if (loadedCategories === 0) {
                            subcategoriesContainer.innerHTML =
                                '<div class="alert alert-danger">Failed to load subcategories. Please try again.</div>';
                        }
                    });
            });
        });
    });

    categoryCheckboxes.forEach(checkbox => {
        if (checkbox.checked) {
            const event = new Event('change');
            checkbox.dispatchEvent(event);
        }
    });
});