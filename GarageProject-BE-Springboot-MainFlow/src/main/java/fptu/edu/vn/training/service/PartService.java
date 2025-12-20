package fptu.edu.vn.training.service;

import fptu.edu.vn.training.model.request.PartCreateRequest;
import fptu.edu.vn.training.model.request.PartFilterRequest;
import fptu.edu.vn.training.model.request.PartUpdateRequest;
import fptu.edu.vn.training.model.response.PagedResult;
import fptu.edu.vn.training.model.response.PartDetailResponse;
import fptu.edu.vn.training.model.response.PartListItemResponse;
import fptu.edu.vn.training.model.response.PartSelectResponse;

import java.util.List;

public interface PartService {

    PagedResult<PartListItemResponse> getPaging(PartFilterRequest filter);

    PartDetailResponse getById(Integer id);

    Integer create(PartCreateRequest request);

    void update(Integer id, PartUpdateRequest request);

    void delete(Integer id);

    boolean checkCodeExists(String partCode, Integer excludeId);

    List<PartSelectResponse> getAllForSelect();
}
