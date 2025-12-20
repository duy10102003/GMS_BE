package fptu.edu.vn.training.model.response;

import java.math.BigDecimal;

public class PartDetailResponse {
    private Integer partId;
    private String partName;
    private String partCode;
    private Integer partQuantity;
    private String partUnit;
    private Integer partCategoryId;
    private String partCategoryName;
    private String partCategoryCode;
    private BigDecimal partPrice;
    private Integer warrantyMonth;

    public Integer getPartId() {
        return partId;
    }

    public void setPartId(Integer partId) {
        this.partId = partId;
    }

    public String getPartName() {
        return partName;
    }

    public void setPartName(String partName) {
        this.partName = partName;
    }

    public String getPartCode() {
        return partCode;
    }

    public void setPartCode(String partCode) {
        this.partCode = partCode;
    }

    public Integer getPartQuantity() {
        return partQuantity;
    }

    public void setPartQuantity(Integer partQuantity) {
        this.partQuantity = partQuantity;
    }

    public String getPartUnit() {
        return partUnit;
    }

    public void setPartUnit(String partUnit) {
        this.partUnit = partUnit;
    }

    public Integer getPartCategoryId() {
        return partCategoryId;
    }

    public void setPartCategoryId(Integer partCategoryId) {
        this.partCategoryId = partCategoryId;
    }

    public String getPartCategoryName() {
        return partCategoryName;
    }

    public void setPartCategoryName(String partCategoryName) {
        this.partCategoryName = partCategoryName;
    }

    public String getPartCategoryCode() {
        return partCategoryCode;
    }

    public void setPartCategoryCode(String partCategoryCode) {
        this.partCategoryCode = partCategoryCode;
    }

    public BigDecimal getPartPrice() {
        return partPrice;
    }

    public void setPartPrice(BigDecimal partPrice) {
        this.partPrice = partPrice;
    }

    public Integer getWarrantyMonth() {
        return warrantyMonth;
    }

    public void setWarrantyMonth(Integer warrantyMonth) {
        this.warrantyMonth = warrantyMonth;
    }
}
