package fptu.edu.vn.training.model.request;

import java.math.BigDecimal;

public class PartUpdateRequest {
    private String partName;
    private String partCode;
    private Integer partQuantity;
    private String partUnit;
    private Integer partCategoryId;
    private BigDecimal partPrice;
    private Integer warrantyMonth;

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
