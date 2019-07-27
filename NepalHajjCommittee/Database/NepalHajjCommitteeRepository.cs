using NepalHajjCommittee.Database.EDMX;

namespace NepalHajjCommittee.Database
{
    public class NepalHajjCommitteeRepository : INepalHajjCommitteeRepository
    {
        #region Private field members
        private NepalHajjCommitteeConnectionString _dataContext;
        private Repository<Room> _roomRepository;
        private Repository<Bed> _bedRepository;
        private Repository<HaajiGroup> _haajiGroupRepository;
        private Repository<Batch> _batchRepository;
        private Repository<Person> _personRepository;
        #endregion

        #region CTOR
        public NepalHajjCommitteeRepository()
        {
            _dataContext = _dataContext ?? (_dataContext = new NepalHajjCommitteeConnectionString());
            _dataContext.Configuration.AutoDetectChangesEnabled = false;
            _dataContext.Configuration.ValidateOnSaveEnabled = false;
        }
        #endregion

        #region Public Repository Properties

        public Repository<Room> RoomRepository => _roomRepository ?? (_roomRepository = new Repository<Room>(_dataContext));
        public Repository<Bed> BedRepository => _bedRepository ?? (_bedRepository = new Repository<Bed>(_dataContext));
        public Repository<HaajiGroup> HaajiGroupRepository => _haajiGroupRepository ?? (_haajiGroupRepository = new Repository<HaajiGroup>(_dataContext));
        public Repository<Batch> BatchRepository => _batchRepository ?? (_batchRepository = new Repository<Batch>(_dataContext));
        public Repository<Person> PersonRepository => _personRepository ?? (_personRepository = new Repository<Person>(_dataContext));
        #endregion

        #region Public Methods
        public void Commit()
        {
            _dataContext.SaveChanges();
            Refresh();
        }

        private void Refresh()
        {
            _dataContext = new NepalHajjCommitteeConnectionString();
            _dataContext.Configuration.AutoDetectChangesEnabled = false;
            _dataContext.Configuration.ValidateOnSaveEnabled = false;

            _roomRepository = new Repository<Room>(_dataContext);
            _bedRepository = new Repository<Bed>(_dataContext);
            _haajiGroupRepository = new Repository<HaajiGroup>(_dataContext);
            _batchRepository = new Repository<Batch>(_dataContext);
            _personRepository = new Repository<Person>(_dataContext);
        }

        #endregion
    }
}
