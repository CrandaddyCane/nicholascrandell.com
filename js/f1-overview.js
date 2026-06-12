const raceStrip = document.getElementById('raceStrip');
const driverStandingsBody = document.getElementById('driverStandingsBody');

const countryFlags = {
  Australia: '🇦🇺',
  China: '🇨🇳',
  Japan: '🇯🇵',
  Bahrain: '🇧🇭',
  'Saudi Arabia': '🇸🇦',
  USA: '🇺🇸',
  Italy: '🇮🇹',
  Monaco: '🇲🇨',
  Spain: '🇪🇸',
  Canada: '🇨🇦',
  Austria: '🇦🇹',
  UK: '🇬🇧',
  Hungary: '🇭🇺',
  Belgium: '🇧🇪',
  Netherlands: '🇳🇱',
  Azerbaijan: '🇦🇿',
  Singapore: '🇸🇬',
  Mexico: '🇲🇽',
  Brazil: '🇧🇷',
  Qatar: '🇶🇦',
  UAE: '🇦🇪'
};

const nationalityFlags = {
  Australian: '🇦🇺',
  British: '🇬🇧',
  Dutch: '🇳🇱',
  Monegasque: '🇲🇨',
  German: '🇩🇪',
  Spanish: '🇪🇸',
  French: '🇫🇷',
  Italian: '🇮🇹',
  Finnish: '🇫🇮',
  Mexican: '🇲🇽',
  Canadian: '🇨🇦',
  Japanese: '🇯🇵',
  Thai: '🇹🇭',
  Chinese: '🇨🇳',
  Brazilian: '🇧🇷',
  Argentine: '🇦🇷',
  American: '🇺🇸',
  Danish: '🇩🇰',
  'New Zealander': '🇳🇿'
};

const cancelledRaces = [
  {
    raceName: 'Bahrain Grand Prix',
    date: '2026-04-12',
    country: 'Bahrain',
    cancelled: true
  },
  {
    raceName: 'Saudi Arabian Grand Prix',
    date: '2026-04-19',
    country: 'Saudi Arabia',
    cancelled: true
  }
];

function formatRaceDate(dateString) {
  const date = new Date(`${dateString}T12:00:00Z`);
  const month = date.getUTCMonth() + 1;
  const day = date.getUTCDate();

  return `${month}/${day}`;
}

function getNextRaceIndex(races) {
  const today = new Date();

  return races.findIndex((race) => {
    if (race.cancelled) return false;

    const raceDate = new Date(`${race.date}T23:59:59Z`);
    return raceDate >= today;
  });
}

function normalizeRace(race) {
  return {
    raceName: race.raceName,
    date: race.date,
    country: race.Circuit.Location.country,
    cancelled: false
  };
}

async function loadRaceStrip() {
  try {
    const response = await fetch('https://api.jolpi.ca/ergast/f1/2026.json');

    if (!response.ok) {
      throw new Error('Unable to load race calendar.');
    }

    const data = await response.json();
    const apiRaces = data.MRData.RaceTable.Races.map(normalizeRace);

    const races = [...apiRaces, ...cancelledRaces].sort((a, b) => {
      return new Date(a.date) - new Date(b.date);
    });

    const nextRaceIndex = getNextRaceIndex(races);

    raceStrip.innerHTML = races.map((race, index) => {
      const flag = countryFlags[race.country] || '';
      const date = formatRaceDate(race.date);
      const isNext = index === nextRaceIndex;
      const isPast = new Date(`${race.date}T23:59:59Z`) < new Date();

      return `
        <div class="race-marker ${isPast ? 'is-past' : 'is-future'} ${race.cancelled ? 'is-cancelled' : ''}" title="${race.raceName}">
          <div class="race-flag-wrap">
            <div class="race-flag">${flag}</div>
            ${race.cancelled ? '<div class="race-cancelled-x">×</div>' : ''}
          </div>
          <div class="race-date">${date}</div>
          <div class="race-dot ${isNext ? 'is-next' : ''}"></div>
        </div>
      `;
    }).join('');
  } catch (error) {
    raceStrip.innerHTML = '<p class="f1-error">Unable to load race calendar.</p>';
    console.error(error);
  }
}

async function loadDriverStandings() {
  try {
    const response = await fetch('https://api.jolpi.ca/ergast/f1/2026/driverstandings.json');

    if (!response.ok) {
      throw new Error('Unable to load driver standings.');
    }

    const data = await response.json();
    const standingsList = data.MRData.StandingsTable.StandingsLists[0];
  const topTenStandings = standings.slice(0, 10);
    if (!standingsList) {
      driverStandingsBody.innerHTML = `
        <div class="standings-loading">
          No driver standings available yet.
        </div>
      `;
      return;
    }

    const standings = standingsList.DriverStandings;

    driverStandingsBody.innerHTML = topTenstandings.map((standing) => {
      const driver = standing.Driver;
      const team = standing.Constructors?.[0] || standing.Constructor || {};
      const flag = nationalityFlags[driver.nationality] || '';

      return `
        <div class="standings-row">
          <div>${standing.position}</div>
          <div class="change-cell">–</div>
          <div class="driver-cell">
            <span>${flag}</span>
            <span>${driver.familyName}</span>
          </div>
          <div>${team.name || 'Unknown'}</div>
          <div class="points-cell">${standing.points}</div>
        </div>
      `;
    }).join('');
  } catch (error) {
    driverStandingsBody.innerHTML = `
      <div class="standings-loading">
        Unable to load driver standings.
      </div>
    `;
    console.error(error);
  }
}
async function loadPointsTrendChart() {
  const chartCanvas = document.getElementById('pointsTrendChart');

  if (!chartCanvas) return;

  const teamColors = {
    Mercedes: '#27F4D2',
    Ferrari: '#E80020',
    McLaren: '#FF8000',
    'Red Bull': '#3671C6',
    'RB F1 Team': '#6692FF',
    Williams: '#005AFF',
    'Alpine F1 Team': '#0093CC',
    'Haas F1 Team': '#B6BABD',
    'Aston Martin': '#229971',
    Sauber: '#52E252'
  };

  try {
    const response = await fetch('https://api.jolpi.ca/ergast/f1/2026/results.json?limit=1000');

    if (!response.ok) {
      throw new Error('Unable to load race results.');
    }

    const data = await response.json();
    const races = data.MRData.RaceTable.Races;

    const driverTotals = {};
    const driverTeams = {};
    const raceLabels = [];

    races.forEach((race) => {
      const country = race.Circuit.Location.country;
      const flag = countryFlags[country] || race.raceName;

      raceLabels.push(flag);

      race.Results.forEach((result) => {
        const driver = result.Driver;
        const constructor = result.Constructor;
        const driverCode = driver.code || driver.familyName.slice(0, 3).toUpperCase();
        const teamName = constructor.name;
        const points = Number(result.points);

        if (!driverTotals[driverCode]) {
          driverTotals[driverCode] = {
            currentTotal: 0,
            totals: []
          };
        }

        driverTeams[driverCode] = teamName;
        driverTotals[driverCode].currentTotal += points;
      });

      Object.keys(driverTotals).forEach((driverCode) => {
        driverTotals[driverCode].totals.push(driverTotals[driverCode].currentTotal);
      });
    });

    const topTenDrivers = Object.entries(driverTotals)
      .map(([driverCode, data]) => ({
        driverCode,
        teamName: driverTeams[driverCode],
        totals: data.totals,
        finalPoints: data.totals[data.totals.length - 1] || 0
      }))
      .sort((a, b) => b.finalPoints - a.finalPoints)
      .slice(0, 10);

    const teamLeaderByTeam = {};

    topTenDrivers.forEach((driver) => {
      const currentLeader = teamLeaderByTeam[driver.teamName];

      if (!currentLeader || driver.finalPoints > currentLeader.finalPoints) {
        teamLeaderByTeam[driver.teamName] = driver;
      }
    });

    new Chart(chartCanvas, {
      type: 'line',
      data: {
        labels: raceLabels,
        datasets: topTenDrivers.map((driver) => {
          const isTeamLeader =
            teamLeaderByTeam[driver.teamName]?.driverCode === driver.driverCode;

          return {
            label: driver.driverCode,
            data: driver.totals,
            borderColor: teamColors[driver.teamName] || '#ffffff',
            backgroundColor: teamColors[driver.teamName] || '#ffffff',
            borderWidth: isTeamLeader ? 3 : 2,
            borderDash: isTeamLeader ? [] : [6, 5],
            tension: 0.25,
            pointRadius: 2,
            pointHoverRadius: 5
          };
        })
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: {
          mode: 'nearest',
          intersect: false
        },
        plugins: {
          legend: {
            position: 'right',
            labels: {
              color: '#f4f4f6',
              boxWidth: 28,
              usePointStyle: false
            }
          },
          tooltip: {
            callbacks: {
              label: function(context) {
                return `${context.dataset.label}: ${context.parsed.y} pts`;
              }
            }
          }
        },
        scales: {
          x: {
            ticks: {
              color: '#9ca3af'
            },
            grid: {
              color: 'rgba(255, 255, 255, 0.06)'
            }
          },
          y: {
            beginAtZero: true,
            suggestedMax: 180,
            ticks: {
              color: '#9ca3af'
            },
            grid: {
              color: 'rgba(255, 255, 255, 0.06)'
            }
          }
        }
      }
    });
  } catch (error) {
    console.error(error);
  }
}

function syncChartHeight() {
  const table = document.querySelector('.standings-table');
  const chartCard = document.querySelector('.points-chart-card');

  if (!table || !chartCard) return;

  chartCard.style.height = `${table.offsetHeight}px`;
}

syncChartHeight();
loadRaceStrip();
loadDriverStandings();
loadPointsTrendChart();
