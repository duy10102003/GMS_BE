package fptu.edu.vn.training;

import fptu.edu.vn.training.entity.GarageService;
import fptu.edu.vn.training.entity.Part;
import fptu.edu.vn.training.entity.PartCategory;
import fptu.edu.vn.training.entity.Role;
import fptu.edu.vn.training.entity.ServiceTicket;
import fptu.edu.vn.training.entity.User;
import fptu.edu.vn.training.entity.Vehicle;
import jakarta.persistence.EntityManager;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.jdbc.AutoConfigureTestDatabase;
import org.springframework.boot.test.autoconfigure.orm.jpa.DataJpaTest;

import java.math.BigDecimal;
import java.time.LocalDateTime;

@DataJpaTest
@AutoConfigureTestDatabase(replace = AutoConfigureTestDatabase.Replace.NONE)
class ERDIntegrityTest {

    @Autowired
    private EntityManager entityManager;

    @Test
    void insertGraph_respectsForeignKeys() {
        Role role = Role.builder()
                .roleName("Admin")
                .description("Temp role for FK test")
                .build();
        entityManager.persist(role);
        entityManager.flush();

        User user = User.builder()
                .fullName("Test User")
                .email("test@example.com")
                .status("ACTIVE")
                .roleId(role.getRoleId())
                .build();
        entityManager.persist(user);
        entityManager.flush();

        PartCategory category = PartCategory.builder()
                .partCategoryName("Engine Parts")
                .partCategoryCode("ENG")
                .status("ACTIVE")
                .build();
        entityManager.persist(category);
        entityManager.flush();

        Part part = Part.builder()
                .partName("Oil Filter")
                .partCode("OF-001")
                .partQuantity(10)
                .partUnit("pcs")
                .partCategoryId(category.getPartCategoryId())
                .partPrice(BigDecimal.valueOf(199.99))
                .warrantyMonth(12)
                .build();
        entityManager.persist(part);
        entityManager.flush();

        GarageService garageService = GarageService.builder()
                .garageServiceName("Oil Change")
                .garageServicePrice(BigDecimal.valueOf(299.99))
                .build();
        entityManager.persist(garageService);
        entityManager.flush();

        Vehicle vehicle = Vehicle.builder()
                .vehicleName("Toyota Vios")
                .vehicleLicensePlate("XX-1234")
                .make("Toyota")
                .model("Vios")
                .build();
        entityManager.persist(vehicle);
        entityManager.flush();

        ServiceTicket serviceTicket = ServiceTicket.builder()
                .bookingId(null)
                .vehicleId(vehicle.getVehicleId())
                .createdBy(user.getUserId())
                .createdDate(LocalDateTime.now())
                .initialIssue("Routine maintenance")
                .serviceTicketCode("ST-001")
                .serviceTicketStatus((byte) 1)
                .build();
        entityManager.persist(serviceTicket);
        entityManager.flush();
    }
}
