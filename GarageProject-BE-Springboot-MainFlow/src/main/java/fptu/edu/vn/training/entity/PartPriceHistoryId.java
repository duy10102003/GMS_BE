package fptu.edu.vn.training.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;
import java.time.LocalDateTime;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Embeddable
public class PartPriceHistoryId implements Serializable {

    @Column(name = "part_id")
    private Integer partId;

    @Column(name = "date_history")
    private LocalDateTime dateHistory;
}
