CREATE TABLE IF NOT EXISTS sales (
    id         SERIAL PRIMARY KEY,
    store      VARCHAR(50)    NOT NULL,
    product    VARCHAR(100)   NOT NULL,
    quantity   INT            NOT NULL,
    unit_price NUMERIC(10,2)  NOT NULL,
    sold_at    TIMESTAMP      NOT NULL DEFAULT now()
);
