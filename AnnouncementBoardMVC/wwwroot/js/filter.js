document.addEventListener('DOMContentLoaded', function () {
    const categoryCheckboxes = document.querySelectorAll('.category-checkbox');
    const subcategoriesContainer = document.getElementById('subcategoriesContainer');
    const meta = document.querySelector('meta[name="api-base-url"]');
    const API_BASE_URL = meta ? meta.content : '';

    // Add animation to the checkboxes
    categoryCheckboxes.forEach(checkbox => {
        const label = checkbox.nextElementSibling;
        if (label) {
            label.addEventListener('mouseenter', function () {
                this.classList.add('fw-bold');
            });

            label.addEventListener('mouseleave', function () {
                this.classList.remove('fw-bold');
            });
        }
    });

    categoryCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const category = this.dataset.category;

            const selectedCategories = [];
            document.querySelectorAll('.category-checkbox:checked').forEach(cb => {
                selectedCategories.push(cb.value);
            });

            if (selectedCategories.length === 0) {
                subcategoriesContainer.innerHTML = `
                    <div class="text-center text-muted p-4">
                        <i class="fas fa-hand-point-left fa-2x mb-2"></i>
                        <p>Select categories to see available subcategories</p>
                    </div>`;
                return;
            }

            // Create loading indicator
            const loadingDiv = document.createElement('div');
            loadingDiv.className = 'loading-container';
            loadingDiv.innerHTML = `
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="loading-text">Loading subcategories...</p>`;

            subcategoriesContainer.innerHTML = '';
            subcategoriesContainer.appendChild(loadingDiv);

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
                        heading.innerHTML = `<i class="fas fa-folder me-1"></i>${category}`;
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
                                           id="subcategory_${value.replace(/\s+/g, '_')}"
                                           name="SelectedSubCategories"
                                           checked>
                                    <label class="form-check-label" for="subcategory_${value.replace(/\s+/g, '_')}">
                                        ${value}
                                    </label>
                                `;

                                checkboxContainer.appendChild(checkDiv);
                            });
                        } else {
                            checkboxContainer.innerHTML = '<p class="text-muted"><i class="fas fa-info-circle me-1"></i>No subcategories available</p>';
                        }

                        groupDiv.appendChild(checkboxContainer);
                        subcategoriesContainer.appendChild(groupDiv);

                        // Add fade-in animation to each group
                        setTimeout(() => {
                            groupDiv.style.opacity = '0';
                            groupDiv.style.transition = 'opacity 0.3s ease-in-out';
                            setTimeout(() => {
                                groupDiv.style.opacity = '1';
                            }, 50);
                        }, 0);
                    })
                    .catch(error => {
                        console.error('Error loading subcategories:', error);

                        if (loadedCategories === 0) {
                            subcategoriesContainer.innerHTML = `
                                <div class="alert alert-danger">
                                    <i class="fas fa-exclamation-circle me-2"></i>Failed to load subcategories. Please try again.
                                </div>`;
                        }
                    });
            });
        });
    });

    // Trigger change event for any pre-checked categories
    categoryCheckboxes.forEach(checkbox => {
        if (checkbox.checked) {
            const event = new Event('change');
            checkbox.dispatchEvent(event);
        }
    });
});