// Initialize tooltips
document.addEventListener('DOMContentLoaded', function () {
    // Initialize Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize charts with clean theme
    initializeCharts();

    // Load notifications
    loadNotifications();

    // Setup search functionality
    setupSearch();

    // Auto-refresh data every 30 seconds
    setInterval(loadNotifications, 30000);
});

// Section Navigation
function showSection(sectionId) {
    // Hide all sections
    const sections = document.querySelectorAll('.form-section');
    sections.forEach(section => {
        section.classList.remove('active');
    });

    // Show selected section
    const targetSection = document.getElementById(sectionId);
    if (targetSection) {
        targetSection.classList.add('active');

        // Update page title
        const titles = {
            'dashboard': 'Dashboard Overview',
            'addProduct': 'Add New Product',
            'productsSection': 'Products Management',
            'categoriesSection': 'Categories Management',
            'ordersSection': 'Orders Management',
            'analyticsSection': 'Sales Analytics'
        };

        document.getElementById('pageTitle').textContent = titles[sectionId] || 'Admin Panel';

        // Update active nav link
        const navLinks = document.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.classList.remove('active');
        });

        const targetLink = document.querySelector(`[onclick="showSection('${sectionId}')"]`);
        if (targetLink) {
            targetLink.classList.add('active');
        }

        // If showing analytics, refresh charts
        if (sectionId === 'analyticsSection') {
            setTimeout(initializeCharts, 100);
        }
    }
}

// Initialize Charts with clean theme
function initializeCharts() {
    // Destroy existing charts
    Chart.helpers.each(Chart.instances, function (instance) {
        instance.destroy();
    });

    // Sales Chart
    const salesCtx = document.getElementById('salesChart');
    if (salesCtx) {
        const salesChart = new Chart(salesCtx.getContext('2d'), {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Sales ($)',
                    data: [15000, 22000, 18000, 31000, 25000, 42000],
                    borderColor: '#1a1a1a',
                    backgroundColor: 'rgba(26, 26, 26, 0.05)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4,
                    pointBackgroundColor: '#1a1a1a',
                    pointBorderColor: '#ffffff',
                    pointBorderWidth: 2,
                    pointRadius: 4,
                    pointHoverRadius: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        labels: {
                            color: '#1a1a1a',
                            font: {
                                size: 13
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)',
                            drawBorder: false
                        },
                        ticks: {
                            color: '#666666',
                            callback: function (value) {
                                return '$' + value.toLocaleString();
                            }
                        }
                    },
                    x: {
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)',
                            display: false
                        },
                        ticks: {
                            color: '#666666'
                        }
                    }
                }
            }
        });
    }

    // Orders Chart
    const ordersCtx = document.getElementById('ordersChart');
    if (ordersCtx) {
        const ordersChart = new Chart(ordersCtx.getContext('2d'), {
            type: 'bar',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Number of Orders',
                    data: [45, 52, 48, 61, 55, 72],
                    backgroundColor: 'rgba(40, 167, 69, 0.7)',
                    borderColor: '#28a745',
                    borderWidth: 1,
                    borderRadius: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        labels: {
                            color: '#1a1a1a',
                            font: {
                                size: 13
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)',
                            drawBorder: false
                        },
                        ticks: {
                            color: '#666666'
                        }
                    },
                    x: {
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)',
                            display: false
                        },
                        ticks: {
                            color: '#666666'
                        }
                    }
                }
            }
        });
    }

    // Revenue Distribution Chart
    const revenueCtx = document.getElementById('revenueChart');
    if (revenueCtx) {
        const revenueChart = new Chart(revenueCtx.getContext('2d'), {
            type: 'doughnut',
            data: {
                labels: ['Men', 'Women', 'Kids', 'Accessories'],
                datasets: [{
                    data: [35, 40, 15, 10],
                    backgroundColor: [
                        '#1a1a1a',
                        '#666666',
                        '#999999',
                        '#cccccc'
                    ],
                    borderWidth: 1,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            color: '#666666',
                            font: {
                                size: 12
                            }
                        }
                    }
                }
            }
        });
    }

    // Categories Chart
    const categoriesCtx = document.getElementById('categoriesChart');
    if (categoriesCtx) {
        const categoriesChart = new Chart(categoriesCtx.getContext('2d'), {
            type: 'pie',
            data: {
                labels: ['Clothing', 'Electronics', 'Footwear', 'Accessories'],
                datasets: [{
                    data: [40, 25, 20, 15],
                    backgroundColor: [
                        '#1a1a1a',
                        '#666666',
                        '#999999',
                        '#cccccc'
                    ],
                    borderWidth: 1,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            color: '#666666',
                            font: {
                                size: 12
                            }
                        }
                    }
                }
            }
        });
    }
}

// Setup Search Functionality
function setupSearch() {
    // Product Search
    const productSearch = document.getElementById('productSearch');
    if (productSearch) {
        productSearch.addEventListener('keyup', function () {
            const filter = this.value.toLowerCase();
            const rows = document.querySelectorAll('#productsTable tbody tr');

            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(filter) ? '' : 'none';
            });
        });
    }

    // Category Search
    const categorySearch = document.getElementById('categorySearch');
    if (categorySearch) {
        categorySearch.addEventListener('keyup', function () {
            const filter = this.value.toLowerCase();
            const rows = document.querySelectorAll('#categoriesTable tbody tr');

            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(filter) ? '' : 'none';
            });
        });
    }

    // Order Search
    const orderSearch = document.getElementById('orderSearch');
    if (orderSearch) {
        orderSearch.addEventListener('keyup', function () {
            const filter = this.value.toLowerCase();
            const rows = document.querySelectorAll('#ordersTable tbody tr');

            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(filter) ? '' : 'none';
            });
        });
    }
}

// Load Notifications
function loadNotifications() {
    // This is a simulation - in real app, fetch from API
    const notifications = [
        { id: 1, message: 'New order #1234 received', type: 'order', time: '2 min ago' },
        { id: 2, message: 'Product "Nike Air Max" is low in stock', type: 'stock', time: '1 hour ago' },
        { id: 3, message: '3 pending orders need approval', type: 'alert', time: '3 hours ago' },
        { id: 4, message: 'Weekly sales report is ready', type: 'report', time: '1 day ago' }
    ];

    const notifList = document.getElementById('notifList');
    const notifCount = document.getElementById('notifCount');

    if (notifList) {
        notifList.innerHTML = '<li class="dropdown-header">Notifications</li>';

        notifications.forEach(notif => {
            const li = document.createElement('li');
            li.className = 'dropdown-item';
            li.innerHTML = `
                <div class="d-flex align-items-start">
                    <div class="me-3">
                        <i class="fas fa-bell text-${notif.type === 'order' ? 'success' : notif.type === 'stock' ? 'warning' : 'info'}"></i>
                    </div>
                    <div>
                        <div class="small">${notif.message}</div>
                        <div class="text-muted" style="font-size: 0.75rem;">${notif.time}</div>
                    </div>
                </div>
            `;
            notifList.appendChild(li);
        });

        notifList.innerHTML += '<li><hr class="dropdown-divider"></li>';
        notifList.innerHTML += '<li><a class="dropdown-item text-center" href="#">View all notifications</a></li>';
    }

    if (notifCount) {
        notifCount.textContent = notifications.length;
    }
}

// Export Data Function
function exportData() {
    showToast('Preparing data for export...', 'info');

    // Simulate export process
    setTimeout(() => {
        // Create CSV content
        let csvContent = "data:text/csv;charset=utf-8,";

        // Add headers
        csvContent += "Product,Category,Price,Status\r\n";

        // Add product data
        const products = [
            { name: "Nike Air Max", category: "Footwear", price: 129.99, status: "Active" },
            { name: "Adidas T-Shirt", category: "Clothing", price: 29.99, status: "Active" },
            { name: "Apple Watch", category: "Electronics", price: 399.99, status: "Active" }
        ];

        products.forEach(product => {
            csvContent += `${product.name},${product.category},${product.price},${product.status}\r\n`;
        });

        // Create download link
        const encodedUri = encodeURI(csvContent);
        const link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", "wear_ease_export_" + new Date().toISOString().split('T')[0] + ".csv");
        document.body.appendChild(link);

        // Trigger download
        link.click();
        document.body.removeChild(link);

        showToast('Data exported successfully!', 'success');
    }, 1000);
}

// Show Toast Notification
function showToast(message, type = 'info') {
    const container = document.getElementById('toastContainer');

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;

    toast.innerHTML = `
        <div class="d-flex justify-content-between align-items-start">
            <div>
                <strong>${type === 'success' ? 'Success!' : type === 'error' ? 'Error!' : 'Info'}</strong>
                <div class="mt-1">${message}</div>
            </div>
            <button type="button" class="btn-close btn-close-sm" onclick="this.parentElement.parentElement.remove()"></button>
        </div>
    `;

    container.appendChild(toast);

    // Auto remove after 5 seconds
    setTimeout(() => {
        if (toast.parentNode) {
            toast.remove();
        }
    }, 5000);
}

// Initialize everything when page loads
window.onload = function () {
    // Show welcome message
    setTimeout(() => {
        showToast('Welcome back, Administrator!', 'info');
    }, 1000);
};