using FikaGg.Api.Common.Enums;

namespace FikaGg.Api.Features.JoinEvent;

public static class Validate
{
	public static void DTOJoinEvent(DTOJoinEvent dto)
	{
		if (Enum.IsDefined(typeof(ParticipantStatus), dto.Status))
		{
			throw new ArgumentException(
				$"['{nameof(dto.Status)}'] contains a value that does not exist in enum ['{nameof(ParticipantStatus)}'].");
		}
	}

}
