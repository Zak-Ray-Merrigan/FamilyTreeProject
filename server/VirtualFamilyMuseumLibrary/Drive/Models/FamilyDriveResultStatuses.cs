namespace VirtualFamilyMuseumLibrary.Drive.Models
{
    public enum FamilyDriveResultStatuses
    {
        Success,

        // Predictable failures: the client's fault, not the domain's — the controller can inspect
        // these instead of catching an exception, since nothing exceptional happened.
        UnsupportedContentType,
        IllegitimateContent
    }
}