-- Seed Data for Demo

-- Users
INSERT INTO users (username, name, email, role) VALUES
('admin', 'Administrator', 'admin@company.com', 'ADMIN'),
('joao.silva', 'João Silva', 'joao.silva@company.com', 'SELLER'),
('maria.santos', 'Maria Santos', 'maria.santos@company.com', 'SELLER');

-- Customers
INSERT INTO customers (code, name, email, phone, document, address, city, state, zip_code, credit_limit) VALUES
('CLI-001', 'Empresa ABC Ltda', 'contato@empresaabc.com.br', '(11) 3456-7890', '12.345.678/0001-90', 'Av. Paulista, 1000', 'São Paulo', 'SP', '01310-100', 50000.00),
('CLI-002', 'Comércio Silva', 'silva@comercio.com.br', '(11) 2345-6789', '23.456.789/0001-12', 'Rua Augusta, 500', 'São Paulo', 'SP', '01304-000', 25000.00),
('CLI-003', 'Indústria Beta', 'compras@industriabeta.com.br', '(19) 3456-7890', '34.567.890/0001-23', 'Rod. Anhanguera, km 100', 'Campinas', 'SP', '13100-000', 100000.00);

-- Products
INSERT INTO products (code, barcode, description, category, unit, cost_price, sale_price, stock_quantity, min_stock) VALUES
('PROD-001', '7891234567890', 'Notebook Dell Inspiron 15', 'INFORMATICA', 'UN', 2500.00, 3500.00, 50, 10),
('PROD-002', '7891234567891', 'Mouse Wireless Logitech', 'INFORMATICA', 'UN', 45.00, 89.00, 200, 50),
('PROD-003', '7891234567892', 'Teclado Mecânico RGB', 'INFORMATICA', 'UN', 150.00, 299.00, 100, 20);

-- Sales (December 2024)
INSERT INTO sales (invoice_number, customer_id, seller_id, sale_date, subtotal, discount_amount, total, payment_method, status) VALUES
('NF-2024-001', 1, 2, '2024-12-01 10:30:00', 3500.00, 0, 3500.00, 'CREDIT_CARD', 'COMPLETED'),
('NF-2024-002', 2, 2, '2024-12-02 14:15:00', 487.90, 0, 487.90, 'DEBIT_CARD', 'COMPLETED'),
('NF-2024-003', 3, 3, '2024-12-03 09:00:00', 7500.00, 500, 7000.00, 'BANK_TRANSFER', 'COMPLETED');

-- Sale Items
INSERT INTO sale_items (sale_id, product_id, quantity, unit_price, discount_percent, total) VALUES
(1, 1, 1, 3500.00, 0, 3500.00),
(2, 2, 2, 89.00, 0, 178.00),
(2, 3, 1, 299.00, 0, 299.00),
(3, 1, 2, 3500.00, 0, 7000.00);