package fptu.edu.vn.training.entity;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDateTime;
import java.util.LinkedHashSet;
import java.util.Set;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Entity
@Table(name = "part_stock_out")
public class PartStockOut {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "part_stock_out_id")
    private Integer partStockOutId;

    @Column(name = "part_stock_out_code", nullable = false, length = 255)
    private String partStockOutCode;

    @Column(name = "requested_by", nullable = false)
    private Integer requestedBy;

    @Column(name = "requested_date")
    private LocalDateTime requestedDate;

    @Column(name = "part_stock_stock_out_status", length = 50)
    private String partStockStockOutStatus;

    @Column(name = "part_stock_stock_out_type", length = 50)
    private String partStockStockOutType;

    @Column(name = "note")
    private String note;

    @Column(name = "confirmed_by")
    private Integer confirmedBy;

    @Column(name = "confirmed_date")
    private LocalDateTime confirmedDate;

    @Column(name = "is_deleted", nullable = false)
    @Builder.Default
    private Integer isDeleted = 0;

    @Builder.Default
    @OneToMany(mappedBy = "partStockOut", fetch = FetchType.LAZY, cascade = CascadeType.ALL, orphanRemoval = false)
    private Set<PartStockOutItem> partStockOutItems = new LinkedHashSet<>();
}
