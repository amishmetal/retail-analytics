package com.example.ingestion;

import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Random;

@Service
public class SaleService {

    public record Sale(String store, String product, int quantity, double unitPrice) {}

    private static final List<String> STORES = List.of("Delhi", "Mumbai", "Bengaluru", "Kolkata", "Chennai");
    private static final List<String> PRODUCTS = List.of("Laptop", "Phone", "Headphones", "Keyboard", "Monitor", "Mouse");
    private static final double[] PRICES = {55000, 25000, 3000, 1500, 12000, 800};

    private final JdbcTemplate jdbc;
    private final Random random = new Random();

    public SaleService(JdbcTemplate jdbc) {
        this.jdbc = jdbc;
    }

    public void save(Sale s) {
        if (s.quantity() <= 0 || s.unitPrice() < 0) {
            throw new IllegalArgumentException("quantity must be > 0 and unitPrice >= 0");
        }
        jdbc.update("INSERT INTO sales(store, product, quantity, unit_price) VALUES (?,?,?,?)",
                s.store(), s.product(), s.quantity(), s.unitPrice());
    }

    /** Simulates live sales feed from stores every 3 seconds. */
    @Scheduled(fixedRate = 3000, initialDelay = 5000)
    public void generateSale() {
        int p = random.nextInt(PRODUCTS.size());
        save(new Sale(STORES.get(random.nextInt(STORES.size())),
                PRODUCTS.get(p), 1 + random.nextInt(3), PRICES[p]));
    }
}
