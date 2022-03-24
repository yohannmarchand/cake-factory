namespace CakeMachine.Fabrication.Elements
{
    internal class ConformeEqualityComparer : IEqualityComparer<IConforme>
    {
        public bool Equals(IConforme? x, IConforme? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.PlatSousJacent.Equals(y.PlatSousJacent);
        }

        public int GetHashCode(IConforme obj) => obj.PlatSousJacent.GetHashCode();
    }
}
