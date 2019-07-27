using NepalHajjCommittee.Database.EDMX;

namespace NepalHajjCommittee.Database
{
    public interface INepalHajjCommitteeRepository
    {
        Repository<Room> RoomRepository { get; }
        Repository<Bed> BedRepository { get; }
        Repository<HaajiGroup> HaajiGroupRepository { get; }
        Repository<Batch> BatchRepository { get; }
        Repository<Person> PersonRepository { get; }

        void Commit();
    }
}
