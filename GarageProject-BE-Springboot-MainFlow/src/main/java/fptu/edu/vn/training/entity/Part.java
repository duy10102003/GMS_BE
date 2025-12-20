package fptu.edu.vn.training.entity;

import fptu.edu.vn.training.enums.PartStatus;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.math.BigDecimal;
import java.util.LinkedHashSet;
import java.util.Set;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Entity
@Table(name = "part")
public class Part {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "part_id")
    private Integer partId;

    @Column(name = "part_name", nullable = false, length = 100)
    private String partName;

    @Column(name = "part_code", nullable = false, length = 20)
    private String partCode;

    @Column(name = "part_quantity", nullable = false)
    private Integer partQuantity;

    @Column(name = "part_unit", nullable = false, length = 20)
    private String partUnit;

    @Column(name = "part_category_id", nullable = false)
    private Integer partCategoryId;

    @Column(name = "part_price", precision = 24, scale = 2)
    private BigDecimal partPrice;

    @Column(name = "selling_price", precision = 24, scale = 2)
    private BigDecimal sellingPrice;

    @Enumerated(EnumType.STRING)
    @Column(name = "status", length = 20)
    private PartStatus status;

    @Column(name = "warranty_month")
    private Integer warrantyMonth;

    @Column(name = "is_deleted", nullable = false)
    @Builder.Default
    private Integer isDeleted = 0;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "part_category_id", insertable = false, updatable = false)
    private PartCategory partCategory;

    @Builder.Default
    @OneToMany(mappedBy = "part", fetch = FetchType.LAZY, cascade = CascadeType.ALL, orphanRemoval = false)
    private Set<ServiceTicketDetail> serviceTicketDetails = new LinkedHashSet<>();

    @Builder.Default
    @OneToMany(mappedBy = "part", fetch = FetchType.LAZY, cascade = CascadeType.ALL, orphanRemoval = false)
    private Set<PartStockOutItem> partStockOutItems = new LinkedHashSet<>();

    @Builder.Default
    @OneToMany(mappedBy = "part", fetch = FetchType.LAZY, cascade = CascadeType.ALL, orphanRemoval = false)
    private Set<PartPriceHistory> partPriceHistories = new LinkedHashSet<>();
}
