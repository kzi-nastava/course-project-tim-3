 using MongoDB.Driver;
 namespace Hospital
 {
     class Hospital
     {
        private MongoClient dbClient = new MongoClient("mongodb://root:root@localhost:27017"); // TODO: move this
     }
 }