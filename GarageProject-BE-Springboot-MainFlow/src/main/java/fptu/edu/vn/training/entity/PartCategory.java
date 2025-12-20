package fptu.edu.vn.training.entity;

import fptu.edu.vn.training.enums.PartCategoryStatus;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
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

import java.util.LinkedHashSet;
import java.util.Set;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Entity
@Table(name = "part_category")
public class PartCategory {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "part_category_id")
    private Integer partCategoryId;

    @Column(name = "part_category_name", length = 100)
    private String partCategoryName;

    @Column(name = "part_category_code", length = 20)
    private String partCategoryCode;

    @Column(name = "part_category_discription", length = 255)
    private String partCategoryDiscription;

    @Column(name = "part_category_phone", length = 50)
    private String partCategoryPhone;

    @Enumerated(EnumType.STRING)
    @Column(name = "status", length = 20)
    @Builder.Default
    private PartCategoryStatus status = PartCategoryStatus.ACTIVE;

    @Builder.Default
    @OneToMany(mappedBy = "partCategory", fetch = FetchType.LAZY, cascade = CascadeType.ALL, orphanRemoval = false)
    private Set<Part> parts = new LinkedHashSet<>();
}
