const API_CATEGORIES = '/api/categories';
const API_PRODUCTS = '/api/products';

let selectedCategoryId = null;

const statusEl = document.getElementById('statusMessage');
const categoriesTbody = document.getElementById('categoriesTableBody');
const productsTbody = document.getElementById('productsTableBody');
const productCategorySelect = document.getElementById('productCategorySelect');
const btnResetFilter = document.getElementById('btnResetProductFilter');

document.addEventListener('DOMContentLoaded', () => {
    loadCategories();
    loadProducts();
    setupEventListeners();
});

function showStatus(text, isError = false) {
    statusEl.textContent = text;
    statusEl.className = `header__status header__status--visible ${isError ? 'header__status--error' : 'header__status--success'
        }`;
    setTimeout(() => {
        statusEl.className = 'header__status';
    }, 4000);
}

async function loadCategories() {
    try {
        const res = await fetch(API_CATEGORIES);
        if (!res.ok) throw new Error('Failed to load categories');
        const categories = await res.json();
        renderCategories(categories);
        updateCategoryDropdown(categories);
    } catch (err) {
        showStatus(err.message, true);
    }
}

function renderCategories(categories) {
    categoriesTbody.innerHTML = '';
    categories.forEach(c => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
      <td><strong>${escapeHtml(c.name)}</strong></td>
      <td>${escapeHtml(c.description || '-')}</td>
      <td class="catalog__actions-cell">
        <button type="button" class="catalog__action-btn catalog__action-btn--filter" onclick="filterProductsByCategory('${c.id}', '${escapeHtml(c.name)}')">Products</button>
        <button type="button" class="catalog__action-btn" onclick="editCategory('${c.id}', '${escapeHtml(c.name)}', '${escapeHtml(c.description || '')}')">Edit</button>
        <button type="button" class="catalog__action-btn catalog__action-btn--delete" onclick="deleteCategory('${c.id}')">Delete</button>
      </td>
    `;
        categoriesTbody.appendChild(tr);
    });
}

function updateCategoryDropdown(categories) {
    productCategorySelect.innerHTML = '<option value="">Select category...</option>';
    categories.forEach(c => {
        const opt = document.createElement('option');
        opt.value = c.id;
        opt.textContent = c.name;
        productCategorySelect.appendChild(opt);
    });
}

async function saveCategory(e) {
    e.preventDefault();
    const id = document.getElementById('categoryId').value;
    const dto = {
        name: document.getElementById('categoryName').value,
        description: document.getElementById('categoryDescription').value
    };

    try {
        const url = id ? `${API_CATEGORIES}/${id}` : API_CATEGORIES;
        const method = id ? 'PUT' : 'POST';

        const res = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });

        if (!res.ok) {
            const errorData = await res.json().catch(() => null);
            throw new Error(errorData?.title || 'Validation error');
        }

        resetCategoryForm();
        await loadCategories();
        showStatus(id ? 'Category updated!' : 'Category created!');
    } catch (err) {
        showStatus(err.message, true);
    }
}

async function deleteCategory(id) {
    if (!confirm('Are you sure you want to delete this category?')) return;
    try {
        const res = await fetch(`${API_CATEGORIES}/${id}`, { method: 'DELETE' });
        if (!res.ok) throw new Error('Failed to delete category');
        await loadCategories();
        await loadProducts();
        showStatus('Category deleted!');
    } catch (err) {
        showStatus(err.message, true);
    }
}

async function deleteAllCategories() {
    if (!confirm('Delete ALL categories?')) return;
    try {
        const res = await fetch(API_CATEGORIES, { method: 'DELETE' });
        if (!res.ok) throw new Error('Failed to delete all categories');
        await loadCategories();
        await loadProducts();
        showStatus('All categories deleted!');
    } catch (err) {
        showStatus(err.message, true);
    }
}

window.editCategory = function (id, name, desc) {
    document.getElementById('categoryId').value = id;
    document.getElementById('categoryName').value = name;
    document.getElementById('categoryDescription').value = desc;
    document.getElementById('btnSaveCategory').textContent = 'Update';
    document.getElementById('btnCancelCategory').classList.remove('catalog__btn--hidden');
};

function resetCategoryForm() {
    document.getElementById('categoryId').value = '';
    document.getElementById('categoryForm').reset();
    document.getElementById('btnSaveCategory').textContent = 'Add Category';
    document.getElementById('btnCancelCategory').classList.add('catalog__btn--hidden');
}

async function loadProducts() {
    try {
        let url = API_PRODUCTS;
        if (selectedCategoryId) {
            url = `${API_CATEGORIES}/${selectedCategoryId}/products`;
        }

        const res = await fetch(url);
        if (!res.ok) throw new Error('Failed to load products');
        const products = await res.json();
        renderProducts(products);
    } catch (err) {
        showStatus(err.message, true);
    }
}

function renderProducts(products) {
    productsTbody.innerHTML = '';
    products.forEach(p => {
        const hasDiscount = p.appliedDiscountPercentage > 0;
        const priceDisplay = hasDiscount
            ? `<s style="color: var(--gray-600);">$${p.originalPrice}</s> <strong>$${p.finalPrice}</strong> <span class="catalog__discount">-${p.appliedDiscountPercentage}%</span>`
            : `<strong>$${p.originalPrice}</strong>`;

        const tr = document.createElement('tr');
        tr.innerHTML = `
      <td><strong>${escapeHtml(p.name)}</strong></td>
      <td>${priceDisplay}</td>
      <td>${p.stockQuantity}</td>
      <td class="catalog__actions-cell">
        <button type="button" class="catalog__action-btn" onclick="editProduct('${p.id}', '${escapeHtml(p.name)}', ${p.originalPrice}, ${p.stockQuantity}, '${p.categoryId}')">Edit</button>
        <button type="button" class="catalog__action-btn catalog__action-btn--delete" onclick="deleteProduct('${p.id}')">Delete</button>
      </td>
    `;
        productsTbody.appendChild(tr);
    });
}

window.filterProductsByCategory = function (catId, catName) {
    selectedCategoryId = catId;
    btnResetFilter.classList.remove('catalog__btn--hidden');
    btnResetFilter.textContent = `Clear Filter (${catName})`;
    loadProducts();
};

async function saveProduct(e) {
    e.preventDefault();
    const id = document.getElementById('productId').value;
    const dto = {
        name: document.getElementById('productName').value,
        price: parseFloat(document.getElementById('productPrice').value),
        stockQuantity: parseInt(document.getElementById('productStock').value, 10),
        categoryId: document.getElementById('productCategorySelect').value
    };

    try {
        const url = id ? `${API_PRODUCTS}/${id}` : API_PRODUCTS;
        const method = id ? 'PUT' : 'POST';

        const res = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });

        if (!res.ok) {
            const errorData = await res.json().catch(() => null);
            throw new Error(errorData?.title || 'Validation error');
        }

        resetProductForm();
        await loadProducts();
        showStatus(id ? 'Product updated!' : 'Product created!');
    } catch (err) {
        showStatus(err.message, true);
    }
}

async function deleteProduct(id) {
    if (!confirm('Are you sure you want to delete this product?')) return;
    try {
        const res = await fetch(`${API_PRODUCTS}/${id}`, { method: 'DELETE' });
        if (!res.ok) throw new Error('Failed to delete product');
        await loadProducts();
        showStatus('Product deleted!');
    } catch (err) {
        showStatus(err.message, true);
    }
}

async function deleteAllProducts() {
    if (!confirm('Delete ALL products?')) return;
    try {
        const res = await fetch(API_PRODUCTS, { method: 'DELETE' });
        if (!res.ok) throw new Error('Failed to delete all products');
        await loadProducts();
        showStatus('All products deleted!');
    } catch (err) {
        showStatus(err.message, true);
    }
}

window.editProduct = function (id, name, price, stock, categoryId) {
    document.getElementById('productId').value = id;
    document.getElementById('productName').value = name;
    document.getElementById('productPrice').value = price;
    document.getElementById('productStock').value = stock;
    document.getElementById('productCategorySelect').value = categoryId;
    document.getElementById('btnSaveProduct').textContent = 'Update';
    document.getElementById('btnCancelProduct').classList.remove('catalog__btn--hidden');
};

function resetProductForm() {
    document.getElementById('productId').value = '';
    document.getElementById('productForm').reset();
    document.getElementById('btnSaveProduct').textContent = 'Add Product';
    document.getElementById('btnCancelProduct').classList.add('catalog__btn--hidden');
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function setupEventListeners() {
    document.getElementById('categoryForm').addEventListener('submit', saveCategory);
    document.getElementById('btnCancelCategory').addEventListener('click', resetCategoryForm);
    document.getElementById('btnDeleteAllCategories').addEventListener('click', deleteAllCategories);

    document.getElementById('productForm').addEventListener('submit', saveProduct);
    document.getElementById('btnCancelProduct').addEventListener('click', resetProductForm);
    document.getElementById('btnDeleteAllProducts').addEventListener('click', deleteAllProducts);

    btnResetFilter.addEventListener('click', () => {
        selectedCategoryId = null;
        btnResetFilter.classList.add('catalog__btn--hidden');
        loadProducts();
    });
}