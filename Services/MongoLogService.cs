using MongoDB.Bson;
using MongoDB.Driver;

namespace GestionAcademica.Services
{
    public class MongoLogService
    {
        private static MongoLogService? _instance;
        private static readonly object _lock = new();
        private readonly IMongoCollection<BsonDocument> _collection;

        private MongoLogService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("GestionAcademicaLogs");
            _collection = database.GetCollection<BsonDocument>("Actividad");
        }

        public static MongoLogService Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new MongoLogService();
                    return _instance;
                }
            }
        }

        public async Task RegistrarAsync(string accion, string detalle)
        {
            var doc = new BsonDocument
            {
                { "accion", accion },
                { "detalle", detalle },
                { "fecha", DateTime.Now }
            };
            await _collection.InsertOneAsync(doc);
        }
    }
}