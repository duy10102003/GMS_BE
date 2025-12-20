package fptu.edu.vn.training.repository;

import fptu.edu.vn.training.entity.Part;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface PartRepository extends JpaRepository<Part, Integer> {

    Page<Part> findAllByIsDeleted(Integer isDeleted, Pageable pageable);

    Optional<Part> findByPartIdAndIsDeleted(Integer id, Integer isDeleted);

    boolean existsByPartCodeAndIsDeleted(String partCode, Integer isDeleted);

    boolean existsByPartCodeAndPartIdNotAndIsDeleted(String partCode, Integer partId, Integer isDeleted);

    List<Part> findByIsDeletedOrderByPartNameAsc(Integer isDeleted);

    @Query("SELECT p FROM Part p WHERE p.isDeleted = 0 AND " +
           "(LOWER(p.partName) LIKE LOWER(CONCAT('%', :keyword, '%')) " +
           "OR LOWER(p.partCode) LIKE LOWER(CONCAT('%', :keyword, '%')))")
    Page<Part> searchForSelect(@Param("keyword") String keyword, Pageable pageable);
}
