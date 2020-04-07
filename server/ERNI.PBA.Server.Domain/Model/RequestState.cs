namespace ERNI.PBA.Server.Domain.Model
{
    public enum RequestState
    {
        Pending = 0,
        ApprovedBySuperior = 1,
        Approved = 2,
        Rejected = 3
    }
}