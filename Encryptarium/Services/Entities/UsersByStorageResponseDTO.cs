namespace BusinessLogic.Entities;

public class UsersByStorageResponseDTO
{
    public List<UserDTO> Users { get; set; } = new List<UserDTO>();
    public List<RightForGroupDTO> Rights { get; set; } = new List<RightForGroupDTO>();
}
