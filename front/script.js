document.addEventListener('DOMContentLoaded', () => {
    // API base URL - change this to match your API Gateway URL
    const API_BASE_URL = '/api';  // Используем относительный путь для API

    // Tab switching functionality
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');

    tabBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            const tabId = btn.getAttribute('data-tab');
            
            // Remove active class from all buttons and tabs
            tabBtns.forEach(b => b.classList.remove('active'));
            tabContents.forEach(t => t.classList.remove('active'));
            
            // Add active class to current button and tab
            btn.classList.add('active');
            document.getElementById(`${tabId}-tab`).classList.add('active');
        });
    });

    // Notification system
    function showNotification(message, type) {
        // Remove any existing notification
        const existingNotification = document.querySelector('.notification');
        if (existingNotification) {
            existingNotification.remove();
        }

        // Create new notification
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;
        document.body.appendChild(notification);

        // Show notification
        setTimeout(() => {
            notification.classList.add('show');
        }, 10);

        // Hide notification after 3 seconds
        setTimeout(() => {
            notification.classList.remove('show');
            setTimeout(() => {
                notification.remove();
            }, 300);
        }, 3000);
    }

    // Helper function for API calls
    async function apiCall(endpoint, method = 'GET', params = {}) {
        try {
            let url = `${API_BASE_URL}${endpoint}`;
            
            // For GET requests, append query parameters to URL
            if (method === 'GET' && Object.keys(params).length > 0) {
                url += '?' + new URLSearchParams(params).toString();
            }
            
            const options = {
                method,
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                mode: 'cors', // Явно указываем режим CORS
                credentials: 'omit' // Не отправляем куки
            };
            
            // For POST requests, add body
            if (method === 'POST') {
                url += '?' + new URLSearchParams(params).toString();
            }
            
            console.log(`Calling API: ${url}`, options);
            const response = await fetch(url, options);
            console.log(`API Response:`, response);
            
            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || `API call failed with status: ${response.status}`);
            }
            
            // Check if response is empty
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                return await response.json();
            }
            
            return {};
        } catch (error) {
            console.error('API Error:', error);
            showNotification(error.message, 'error');
            throw error;
        }
    }

    // Account Management
    const createAccountBtn = document.getElementById('createAccount');
    const checkBalanceBtn = document.getElementById('checkBalance');
    const topupAccountBtn = document.getElementById('topupAccount');
    const userIdInput = document.getElementById('userId');
    const topupAmountInput = document.getElementById('topupAmount');
    const balanceAmount = document.getElementById('balance-amount');

    // Create Account
    createAccountBtn.addEventListener('click', async () => {
        const userId = userIdInput.value.trim();
        
        if (!userId) {
            showNotification('Please enter a user ID', 'error');
            return;
        }
        
        try {
            const result = await apiCall('/payments/create', 'POST', { userId });
            showNotification('Account created successfully!', 'success');
            
            // Update balance display
            if (result && result.balance !== undefined) {
                balanceAmount.textContent = `$${result.balance.toFixed(2)}`;
            }
        } catch (error) {
            console.error('Error creating account:', error);
        }
    });

    // Check Balance
    checkBalanceBtn.addEventListener('click', async () => {
        const userId = userIdInput.value.trim();
        
        if (!userId) {
            showNotification('Please enter a user ID', 'error');
            return;
        }
        
        try {
            const result = await apiCall('/payments/balance', 'GET', { userId });
            
            if (result && result.balance !== undefined) {
                balanceAmount.textContent = `$${result.balance.toFixed(2)}`;
                showNotification('Balance retrieved successfully!', 'success');
            }
        } catch (error) {
            console.error('Error checking balance:', error);
        }
    });

    // Top Up Account
    topupAccountBtn.addEventListener('click', async () => {
        const userId = userIdInput.value.trim();
        const amount = parseFloat(topupAmountInput.value);
        
        if (!userId) {
            showNotification('Please enter a user ID', 'error');
            return;
        }
        
        if (isNaN(amount) || amount <= 0) {
            showNotification('Please enter a valid amount', 'error');
            return;
        }
        
        try {
            const result = await apiCall('/payments/topup', 'POST', { userId, amount });
            
            if (result && result.balance !== undefined) {
                balanceAmount.textContent = `$${result.balance.toFixed(2)}`;
                showNotification(`Account topped up with $${amount.toFixed(2)}!`, 'success');
                topupAmountInput.value = '';
            }
        } catch (error) {
            console.error('Error topping up account:', error);
        }
    });

    // Order Management
    const createOrderBtn = document.getElementById('createOrder');
    const listOrdersBtn = document.getElementById('listOrders');
    const getOrderBtn = document.getElementById('getOrder');
    const orderUserIdInput = document.getElementById('orderUserId');
    const orderAmountInput = document.getElementById('orderAmount');
    const orderDescriptionInput = document.getElementById('orderDescription');
    const listUserIdInput = document.getElementById('listUserId');
    const orderIdInput = document.getElementById('orderId');
    const ordersContainer = document.getElementById('orders-container');
    const orderDetailsContainer = document.getElementById('order-details-container');

    // Create Order
    createOrderBtn.addEventListener('click', async () => {
        const userId = orderUserIdInput.value.trim();
        const amount = parseFloat(orderAmountInput.value);
        const description = orderDescriptionInput.value.trim();
        
        if (!userId) {
            showNotification('Please enter a user ID', 'error');
            return;
        }
        
        if (isNaN(amount) || amount <= 0) {
            showNotification('Please enter a valid amount', 'error');
            return;
        }
        
        if (!description) {
            showNotification('Please enter a description', 'error');
            return;
        }
        
        try {
            const result = await apiCall('/orders', 'POST', { userId, amount, description });
            showNotification('Order created successfully!', 'success');
            
            // Clear inputs
            orderAmountInput.value = '';
            orderDescriptionInput.value = '';
            
            // If we have an order ID, display it
            if (result && result.id) {
                showNotification(`Order created with ID: ${result.id}`, 'success');
            }
        } catch (error) {
            console.error('Error creating order:', error);
        }
    });

    // List Orders
    listOrdersBtn.addEventListener('click', async () => {
        const userId = listUserIdInput.value.trim();
        
        if (!userId) {
            showNotification('Please enter a user ID', 'error');
            return;
        }
        
        try {
            const orders = await apiCall('/orders', 'GET', { userId });
            
            if (Array.isArray(orders) && orders.length > 0) {
                displayOrders(orders);
                showNotification(`Found ${orders.length} orders!`, 'success');
            } else {
                ordersContainer.innerHTML = '<p>No orders found for this user</p>';
                showNotification('No orders found', 'error');
            }
        } catch (error) {
            console.error('Error listing orders:', error);
        }
    });

    // Get Order Details
    getOrderBtn.addEventListener('click', async () => {
        const orderId = orderIdInput.value.trim();
        
        if (!orderId) {
            showNotification('Please enter an order ID', 'error');
            return;
        }
        
        try {
            const order = await apiCall(`/orders/${orderId}`, 'GET');
            displayOrderDetails(order);
            showNotification('Order details retrieved!', 'success');
        } catch (error) {
            console.error('Error getting order details:', error);
            orderDetailsContainer.innerHTML = '<p>Order not found</p>';
        }
    });

    // Helper function to display orders
    function displayOrders(orders) {
        if (!orders || orders.length === 0) {
            ordersContainer.innerHTML = '<p>No orders to display</p>';
            return;
        }
        
        let html = '';
        orders.forEach(order => {
            html += `
                <div class="order-item" data-id="${order.id}">
                    <h3>Order #${order.id.substring(0, 8)}...</h3>
                    <p><strong>Amount:</strong> $${order.amount.toFixed(2)}</p>
                    <p><strong>Description:</strong> ${order.description}</p>
                    <p><strong>Status:</strong> <span class="order-status status-${order.status.toLowerCase()}">${order.status}</span></p>
                </div>
            `;
        });
        
        ordersContainer.innerHTML = html;
        
        // Add click event to order items
        document.querySelectorAll('.order-item').forEach(item => {
            item.addEventListener('click', async () => {
                const orderId = item.getAttribute('data-id');
                orderIdInput.value = orderId;
                
                try {
                    const order = await apiCall(`/orders/${orderId}`, 'GET');
                    displayOrderDetails(order);
                } catch (error) {
                    console.error('Error getting order details:', error);
                }
            });
        });
    }

    // Helper function to display order details
    function displayOrderDetails(order) {
        if (!order || !order.id) {
            orderDetailsContainer.innerHTML = '<p>No order details to display</p>';
            return;
        }
        
        const html = `
            <div class="order-detail">
                <p><strong>Order ID:</strong> ${order.id}</p>
                <p><strong>User ID:</strong> ${order.userId}</p>
                <p><strong>Amount:</strong> $${order.amount.toFixed(2)}</p>
                <p><strong>Description:</strong> ${order.description}</p>
                <p><strong>Status:</strong> <span class="order-status status-${order.status.toLowerCase()}">${order.status}</span></p>
            </div>
        `;
        
        orderDetailsContainer.innerHTML = html;
    }

    let socket = null;

    function connectWebSocket() {
        const wsProtocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
        const wsUrl = `${wsProtocol}//${window.location.host}/ws`;
        
        socket = new WebSocket(wsUrl);
        
        socket.onopen = function(e) {
            console.log("WebSocket connection established");
        };
        
        socket.onmessage = function(event) {
            try {
                const data = JSON.parse(event.data);
                if (data.type === 'order_status_changed') {
                    showNotification(`Order ${data.orderId} status changed to ${data.status}`, 'info');
                }
            } catch (error) {
                console.error("Error parsing WebSocket message:", error);
            }
        };
        
        socket.onclose = function(event) {
            if (event.wasClean) {
                console.log(`WebSocket connection closed cleanly, code=${event.code}, reason=${event.reason}`);
            } else {
                console.log('WebSocket connection died');
                setTimeout(connectWebSocket, 5000);
            }
        };
        
        socket.onerror = function(error) {
            console.error(`WebSocket error: ${error.message}`);
        };
    }
    
    connectWebSocket();
}); 