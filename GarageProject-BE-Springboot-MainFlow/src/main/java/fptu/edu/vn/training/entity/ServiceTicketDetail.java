package fptu.edu.vn.training.entity;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
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

import java.util.LinkedHashSet;
import java.util.Set;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@Entity
@Table(name = "service_ticket_detail")
public class ServiceTicketDetail {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "service_ticket_detail_id")
    private Integer serviceTicketDetailId;

    @Column(name = "part_id")
    private Integer partId;

    @Column(name = "garage_service_id")
    private Integer garageServiceId;

    @Column(name = "service_ticket_id")
    private Integer serviceTicketId;

    @Column(name = "quantity")
    private Integer quantity;

    @Column(name = "is_deleted", nullable = false)
    @Builder.Default
    private Integer isDeleted = 0;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "part_id", insertable = false, updatable = false)
    private Part part;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "garage_service_id", insertable = false, updatable = false)
    private GarageService garageService;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "service_ticket_id", insertable = false, updatable = false)
    private ServiceTicket serviceTicket;

    @Builder.Default
    @OneToMany(mappedBy = "serviceTicketDetail", fetch = FetchType.LAZY, cascade = CascadeType.ALL, orphanRemoval = false)
    private Set<Warranty> warranties = new LinkedHashSet<>();
}
