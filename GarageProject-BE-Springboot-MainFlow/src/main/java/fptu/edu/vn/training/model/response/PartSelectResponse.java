package fptu.edu.vn.training.model.response;

public class PartSelectResponse {
    private Integer partId;
    private String partName;
    private String partCode;
    private Integer partQuantity;
    private String partUnit;

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
}
