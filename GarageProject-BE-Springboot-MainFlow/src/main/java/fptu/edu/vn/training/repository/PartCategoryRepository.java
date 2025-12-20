package fptu.edu.vn.training.repository;

import fptu.edu.vn.training.entity.PartCategory;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface PartCategoryRepository extends JpaRepository<PartCategory, Integer> {
}
