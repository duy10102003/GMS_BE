package fptu.edu.vn.training.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.math.BigDecimal;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Entity
@Table(name = "part_stock_out_items")
public class PartStockOutItem {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "part_stock_out_item_id")
    private Integer partStockOutItemId;

    @Column(name = "part_stock_out_id", nullable = false)
    private Integer partStockOutId;

    @Column(name = "part_id", nullable = false)
    private Integer partId;

    @Column(name = "quantity", nullable = false)
    private Integer quantity;

    @Column(name = "stock_out_price", precision = 24, scale = 2)
    private BigDecimal stockOutPrice;

    @Column(name = "is_deleted", nullable = false)
    @Builder.Default
    private Integer isDeleted = 0;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "part_stock_out_id", insertable = false, updatable = false)
    private PartStockOut partStockOut;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "part_id", insertable = false, updatable = false)
    private Part part;
}
