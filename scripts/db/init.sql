-- Database Schema for Legacy Data Modernization
-- PostgreSQL 16+

-- Customers Table
CREATE TABLE IF NOT EXISTS customers (
    id              SERIAL PRIMARY KEY,
    code            VARCHAR(20) NOT NULL UNIQUE,
    name            VARCHAR(100) NOT NULL,
    email           VARCHAR(100),
    phone           VARCHAR(20),
    document        VARCHAR(20),
    address         VARCHAR(200),
    city            VARCHAR(100),
    state           VARCHAR(2),
    zip_code        VARCHAR(10),
    is_active       BOOLEAN NOT NULL DEFAULT true,
    credit_limit    DECIMAL(15, 2) NOT NULL DEFAULT 0,
    current_balance DECIMAL(15, 2) NOT NULL DEFAULT 0,
    created_at      TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP
);

CREATE INDEX idx_customers_code ON customers(code);
CREATE INDEX idx_customers_name ON customers(name);
CREATE INDEX idx_customers_active ON customers(is_active);

-- Products Table
CREATE TABLE IF NOT EXISTS products (
    id              SERIAL PRIMARY KEY,
    code            VARCHAR(20) NOT NULL UNIQUE,
    barcode         VARCHAR(20),
    description     VARCHAR(200) NOT NULL,
    category        VARCHAR(50),
    unit            VARCHAR(5) DEFAULT 'UN',
    cost_price      DECIMAL(15, 2) NOT NULL DEFAULT 0,
    sale_price      DECIMAL(15, 2) NOT NULL DEFAULT 0,
    stock_quantity  DECIMAL(15, 3) NOT NULL DEFAULT 0,
    min_stock       DECIMAL(15, 3) NOT NULL DEFAULT 0,
    max_stock       DECIMAL(15, 3) NOT NULL DEFAULT 0,
    is_active       BOOLEAN NOT NULL DEFAULT true,
    created_at      TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP
);

CREATE INDEX idx_products_code ON products(code);
CREATE INDEX idx_products_barcode ON products(barcode);

-- Users Table
CREATE TABLE IF NOT EXISTS users (
    id              SERIAL PRIMARY KEY,
    username        VARCHAR(50) NOT NULL UNIQUE,
    name            VARCHAR(100) NOT NULL,
    email           VARCHAR(100),
    role            VARCHAR(20) DEFAULT 'SELLER',
    is_active       BOOLEAN NOT NULL DEFAULT true,
    created_at      TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Sales Table
CREATE TABLE IF NOT EXISTS sales (
    id              SERIAL PRIMARY KEY,
    invoice_number  VARCHAR(20) NOT NULL UNIQUE,
    customer_id     INTEGER REFERENCES customers(id),
    seller_id       INTEGER REFERENCES users(id),
    sale_date       TIMESTAMP NOT NULL DEFAULT NOW(),
    subtotal        DECIMAL(15, 2) NOT NULL DEFAULT 0,
    discount_amount DECIMAL(15, 2) NOT NULL DEFAULT 0,
    tax_amount      DECIMAL(15, 2) NOT NULL DEFAULT 0,
    total           DECIMAL(15, 2) NOT NULL DEFAULT 0,
    payment_method  VARCHAR(20) NOT NULL,
    status          VARCHAR(20) NOT NULL DEFAULT 'COMPLETED',
    created_at      TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMP
);

CREATE INDEX idx_sales_date ON sales(sale_date);
CREATE INDEX idx_sales_customer ON sales(customer_id);
CREATE INDEX idx_sales_status ON sales(status);

-- Sale Items Table
CREATE TABLE IF NOT EXISTS sale_items (
    id              SERIAL PRIMARY KEY,
    sale_id         INTEGER NOT NULL REFERENCES sales(id) ON DELETE CASCADE,
    product_id      INTEGER NOT NULL REFERENCES products(id),
    quantity        DECIMAL(15, 3) NOT NULL,
    unit_price      DECIMAL(15, 2) NOT NULL,
    discount_percent DECIMAL(5, 2) NOT NULL DEFAULT 0,
    total           DECIMAL(15, 2) NOT NULL
);

CREATE INDEX idx_sale_items_sale ON sale_items(sale_id);
CREATE INDEX idx_sale_items_product ON sale_items(product_id);