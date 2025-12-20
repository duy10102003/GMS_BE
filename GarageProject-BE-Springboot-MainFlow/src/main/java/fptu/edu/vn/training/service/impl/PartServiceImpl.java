package fptu.edu.vn.training.service.impl;

import fptu.edu.vn.training.entity.Part;
import fptu.edu.vn.training.repository.PartCategoryRepository;
import fptu.edu.vn.training.repository.PartRepository;
import fptu.edu.vn.training.service.PartService;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;
import org.springframework.util.StringUtils;

import java.util.List;

@Service
@RequiredArgsConstructor
@Transactional
public class PartServiceImpl implements PartService {

    private final PartRepository partRepository;
    private final PartCategoryRepository partCategoryRepository;

    @Override
    @Transactional(readOnly = true)
    public Page<Part> getPaging(Pageable pageable) {
        return partRepository.findAllByIsDeleted(0, pageable);
    }

    @Override
    @Transactional(readOnly = true)
    public Part getById(Integer id) {
        return partRepository.findByPartIdAndIsDeleted(id, 0)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Không tìm thấy part."));
    }

    @Override
    public Part create(Part request) {
        validatePayload(request);
        ensureCodeUnique(request.getPartCode(), null);
        ensurePartCategoryExists(request.getPartCategoryId());

        request.setIsDeleted(0);
        return partRepository.save(request);
    }

    @Override
    public Part update(Integer id, Part request) {
        Part existing = getById(id);

        validatePayload(request);
        ensureCodeUnique(request.getPartCode(), id);
        ensurePartCategoryExists(request.getPartCategoryId());

        existing.setPartName(request.getPartName().trim());
        existing.setPartCode(request.getPartCode().trim());
        existing.setPartQuantity(request.getPartQuantity());
        existing.setPartUnit(request.getPartUnit().trim());
        existing.setPartCategoryId(request.getPartCategoryId());
        existing.setPartPrice(request.getPartPrice());
        existing.setWarrantyMonth(request.getWarrantyMonth());

        return partRepository.save(existing);
    }

    @Override
    public void delete(Integer id) {
        Part existing = getById(id);
        existing.setIsDeleted(1);
        partRepository.save(existing);
    }

    @Override
    @Transactional(readOnly = true)
    public boolean checkCodeExists(String partCode, Integer excludeId) {
        if (!StringUtils.hasText(partCode)) {
            return false;
        }

        if (excludeId != null) {
            return partRepository.existsByPartCodeAndPartIdNotAndIsDeleted(partCode.trim(), excludeId, 0);
        }

        return partRepository.existsByPartCodeAndIsDeleted(partCode.trim(), 0);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Part> getAllForSelect() {
        return partRepository.findByIsDeletedOrderByPartNameAsc(0);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Part> searchForSelect(String keyword, int limit) {
        int take = limit > 0 ? limit : 50;
        String searchKeyword = keyword == null ? "" : keyword.trim();
        Page<Part> page = partRepository.searchForSelect(searchKeyword, PageRequest.of(0, take));
        return page.getContent();
    }

    private void validatePayload(Part request) {
        if (!StringUtils.hasText(request.getPartName())) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Tên phụ tùng không được để trống.");
        }

        if (request.getPartName().length() > 100) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Tên phụ tùng không được vượt quá 100 ký tự.");
        }

        if (!StringUtils.hasText(request.getPartCode())) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Mã phụ tùng không được để trống.");
        }

        if (request.getPartCode().length() > 20) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Mã phụ tùng không được vượt quá 20 ký tự.");
        }

        if (request.getPartQuantity() != null && request.getPartQuantity() < 0) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Số lượng tồn kho phải lớn hơn hoặc bằng 0.");
        }

        if (!StringUtils.hasText(request.getPartUnit())) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Đơn vị tính không được để trống.");
        }

        if (request.getPartUnit().length() > 20) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Đơn vị tính không được vượt quá 20 ký tự.");
        }
    }

    private void ensureCodeUnique(String partCode, Integer excludeId) {
        if (checkCodeExists(partCode, excludeId)) {
            throw new ResponseStatusException(HttpStatus.CONFLICT, "Mã phụ tùng đã tồn tại.");
        }
    }

    private void ensurePartCategoryExists(Integer partCategoryId) {
        if (partCategoryId == null || !partCategoryRepository.existsById(partCategoryId)) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Không tìm thấy danh mục phụ tùng.");
        }
    }
}
