using CareerConnect.Data;
using CareerConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.Services;

public class OfferService
{
    private readonly ApplicationDbContext _context;

    public OfferService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Offer> SendOfferAsync(int applicationId, int employerId)
    {
        var application = await _context.Applications.FindAsync(applicationId);
        if (application == null)
            throw new InvalidOperationException("Başvuru bulunamadı.");

        if (application.SeekerId == employerId)
            throw new InvalidOperationException("Kendi başvuruna teklif gönderemezsin.");

        var alreadySent = await _context.Offers.AnyAsync(o => o.ApplicationId == applicationId);
        if (alreadySent)
            throw new InvalidOperationException("Bu başvuruya zaten teklif yapılmış.");

        var offer = new Offer
        {
            ApplicationId = applicationId,
            EmployerId = employerId
        };

        _context.Offers.Add(offer);
        await _context.SaveChangesAsync();

        return offer;
    }

    public async Task<List<Offer>> GetOffersByUserIdAsync(int seekerId)
    {
        var applicationIds = await _context.Applications
            .Where(a => a.SeekerId == seekerId)
            .Select(a => a.Id)
            .ToListAsync();

        return await _context.Offers
            .Where(o => applicationIds.Contains(o.ApplicationId))
            .ToListAsync();
    }
}
