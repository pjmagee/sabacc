using Sabacc.Domain;

namespace Sabacc.Services
{
    public class SabaccSessionService
    {
        private readonly SabaccSessionFactory _sabaccSessionFactory;
        public List<ISabaccSession> Sessions { get; set; }

        public SabaccSessionService(SabaccSessionFactory sabaccSessionFactory)
        {
            _sabaccSessionFactory = sabaccSessionFactory;
            Sessions = new List<ISabaccSession>();
        }

        public async Task<ISabaccSession> Create(Guid playerId, CreateSessionForm createSessionForm)
        {
            var session = Sessions.Find(x => x.PlayerIds.Contains(playerId));

            if (session is null)
            {
                session = _sabaccSessionFactory.Create(createSessionForm);
                await session.JoinSession(playerId, createSessionForm.PlayerName);
                Sessions.Add(session);
            }

            return session;
        }

        public async Task<SpectatorView> GetSpectatorView(Guid sessionId)
        {
            return new SpectatorView();
        }

        public bool IsValidPlayer(Guid sessionId, Guid playerId)
        {
            return Sessions.Exists(session => session.Id.Equals(sessionId) && session.PlayerIds.Contains(playerId));
        }

        public async Task<PlayerViewModel> GetPlayerView(Guid sessionId, Guid playerId)
        {
            var session = Sessions.Find(session => session.Id == sessionId && session.PlayerIds.Contains(playerId))!;
            return session.GetPlayerView(playerId);
        }

        public async Task<IEnumerable<SabaccSessionListItem>> GetSessions()
        {
            return Sessions.Select(session => new SabaccSessionListItem(session)).ToList();
        }

        public async Task SubmitTurn(Guid sessionId, Guid playerId, PlayerState playerState)
        {
            var session = Sessions.Find(sesson => sesson.Id == sessionId && sesson.PlayerIds.Contains(playerId))!;
            await session.PlayerTurn(playerId, playerState).ConfigureAwait(false);
        }
    }
}
