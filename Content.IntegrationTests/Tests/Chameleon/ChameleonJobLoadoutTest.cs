using System.Collections.Generic;
using Content.IntegrationTests.Tests.Interaction;
using Content.Shared.Clothing;
using Content.Shared.Implants;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests.Chameleon;

/// <summary>
/// Ensures all <see cref="IsProbablyRoundStartJob">"round start jobs"</see> have an associated chameleon loadout.
/// </summary>
public sealed class ChameleonJobLoadoutTest : InteractionTest
{
    private static readonly List<ProtoId<JobPrototype>> JobBlacklist =
    [
        // Stalker jobs - Stalker does not use SS14's chameleon outfit system
        "Doctor",
        "Evolver",
        "EvolverPackAlpha",
        "EvolverPackFang",
        "EvolverSymbioteArchon",
        "EvolverSymbioteMedium",
        "HeadDoctor",
        "HeadMilitaryStalker",
        "Magistrate",
        "MayorCity",
        "Military",
        "MilitaryHead",
        "MilitaryOfficer",
        "MilitaryStalker",
        "Police",
        "Priest",
        "Prokuror",
        "sci",
        "sci_decan",
        "sci_rector",
        "Seraphim",
        "SeraphimCherubim",
        "SeraphimHead",
        "Stalker",
        "StalkerAnomalist",
        "StalkerAnomalistGuardian",
        "StalkerBandit",
        "StalkerBartender",
        "StalkerBishop",
        "StalkerClearSky",
        "StalkerClearSkyHead",
        "StalkerClown",
        "StalkerDeserter",
        "StalkerDeserterHead",
        "StalkerDolg",
        "StalkerEcologist",
        "StalkerFreedom",
        "StalkerGreh",
        "StalkerGuide",
        "StalkerHeadBandit",
        "StalkerHeadClearSky",
        "StalkerHeadDolg",
        "StalkerHeadEcologist",
        "StalkerHeadFreedom",
        "StalkerHeadMercenary",
        "StalkerHeadMonolith",
        "StalkerHeadRene",
        "StalkerJaba",
        "StalkerJabaAce",
        "StalkerJournalist",
        "StalkerMercenary",
        "StalkerMilitia",
        "StalkerMilitiaCommander",
        "StalkerMonolith",
        "StalkerNeutral",
        "StalkerPoisk",
        "StalkerPostulant",
        "StalkerProjectOrchestrator",
        "StalkerProjectPawn",
        "StalkerRene",
        "StalkerSBU",
        "StalkerScientist",
        "StalkerTrader",
        "StalkerUN",
        "StalkerVeteran",
        "StalkerZavet",
        "StalkerZavetHead",
    ];

    [Test]
    public Task CheckAllJobs()
    {
        var alljobs = ProtoMan.EnumeratePrototypes<JobPrototype>();

        // Job -> number of references
        Dictionary<ProtoId<JobPrototype>, int> validJobs = new();

        // Only add stuff that actually has clothing! We don't want stuff like AI or borgs.
        foreach (var job in alljobs)
        {
            if (!IsProbablyRoundStartJob(job) || JobBlacklist.Contains(job.ID))
                continue;

            validJobs.Add(job.ID, 0);
        }

        var chameleons = ProtoMan.EnumeratePrototypes<ChameleonOutfitPrototype>();

        foreach (var chameleon in chameleons)
        {
            if (chameleon.Job == null || !validJobs.ContainsKey(chameleon.Job.Value))
                continue;

            validJobs[chameleon.Job.Value] += 1;
        }

        Assert.Multiple(() =>
        {
            foreach (var job in validJobs)
            {
                Assert.That(job.Value, Is.Not.Zero,
                    $"{job.Key} has no chameleonOutfit prototype.");
            }
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Best guess at what a "round start" job is.
    /// </summary>
    private bool IsProbablyRoundStartJob(JobPrototype job)
    {
        return job.StartingGear != null && ProtoMan.HasIndex<RoleLoadoutPrototype>(LoadoutSystem.GetJobPrototype(job.ID));
    }

}
