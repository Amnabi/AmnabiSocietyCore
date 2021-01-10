using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amnabi {
    public enum InterestLevel
    {
        NotInterested,
        Intereseted,
        Self
    }
    public enum DiscourageReason
    {
        None,
        Dirty,
        Disgusting,
        Dangerous,
        Barbaric,
        Immoral,
        Holy,
        Unholy,
        Immodest,
        Improper,
        Impractical
    }
    public enum EncourageReason
    {
        None,
        Moral,
        Tasty,
        Beautiful,
        Healthy,
        Practical,
        Traditional,
        Clean,
    }
    public static class AmnabiIdeaEnumUtil
    {
        public static IEnumerable<EncourageReason> ClothingReasonGood()
        {
            yield return EncourageReason.Beautiful;
            yield return EncourageReason.Moral;
            yield return EncourageReason.Healthy;
            yield return EncourageReason.Practical;
            yield return EncourageReason.Traditional;
            yield return EncourageReason.Clean;
            yield break;
        }
        public static IEnumerable<DiscourageReason> ClothingReasonBad()
        {
            yield return DiscourageReason.Barbaric;
            yield return DiscourageReason.Disgusting;
            yield return DiscourageReason.Immodest;
            yield return DiscourageReason.Immoral;
            yield return DiscourageReason.Unholy;
            yield return DiscourageReason.Improper;
            yield return DiscourageReason.Impractical;
            yield break;
        }

    }

}
