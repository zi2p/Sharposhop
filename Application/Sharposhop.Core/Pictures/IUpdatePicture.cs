using Sharposhop.Core.Model;

namespace Sharposhop.Core.Pictures;

public interface IUpdatePicture : IPicture
{
    void UpdateGamma(Gamma value);
}