import { useMemo, useState } from "react";

type SimpleCalendarProps = {
  bookedDates: string[];
  selectedDate: string | null;
  onSelectDate: (date: string) => void;
};

const monthNames = [
  "Jan",
  "Feb",
  "Mar",
  "Apr",
  "May",
  "Jun",
  "Jul",
  "Aug",
  "Sep",
  "Oct",
  "Nov",
  "Dec",
];

const weekdayNames = ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"];

function formatDateKey(year: number, month: number, day: number) {
  const realMonth = String(month + 1).padStart(2, "0");
  const realDay = String(day).padStart(2, "0");
  return `${year}-${realMonth}-${realDay}`;
}

export default function SimpleCalendar({
  bookedDates,
  selectedDate,
  onSelectDate,
}: SimpleCalendarProps) {
  const [month, setMonth] = useState(11);
  const [year, setYear] = useState(2026);

  const firstDayOfMonth = new Date(year, month, 1).getDay();
  const daysInMonth = new Date(year, month + 1, 0).getDate();

  const calendarCells = useMemo(() => {
    const cells: Array<number | null> = [];

    for (let i = 0; i < firstDayOfMonth; i += 1) {
      cells.push(null);
    }

    for (let day = 1; day <= daysInMonth; day += 1) {
      cells.push(day);
    }

    while (cells.length % 7 !== 0) {
      cells.push(null);
    }

    return cells;
  }, [firstDayOfMonth, daysInMonth]);

  return (
    <div className="simple-calendar">
      <div className="simple-calendar-header">
        <button
          className="simple-calendar-arrow"
          onClick={() => {
            if (month === 0) {
              setMonth(11);
              setYear((prev) => prev - 1);
            } else {
              setMonth((prev) => prev - 1);
            }
          }}
        >
          ‹
        </button>

        <select
          className="simple-calendar-select"
          value={month}
          onChange={(e) => setMonth(Number(e.target.value))}
        >
          {monthNames.map((monthName, index) => (
            <option key={monthName} value={index}>
              {monthName}
            </option>
          ))}
        </select>

        <select
          className="simple-calendar-select"
          value={year}
          onChange={(e) => setYear(Number(e.target.value))}
        >
          {[2026, 2027, 2028].map((yearOption) => (
            <option key={yearOption} value={yearOption}>
              {yearOption}
            </option>
          ))}
        </select>

        <button
          className="simple-calendar-arrow"
          onClick={() => {
            if (month === 11) {
              setMonth(0);
              setYear((prev) => prev + 1);
            } else {
              setMonth((prev) => prev + 1);
            }
          }}
        >
          ›
        </button>
      </div>

      <div className="simple-calendar-weekdays">
        {weekdayNames.map((day) => (
          <div key={day} className="simple-calendar-weekday">
            {day}
          </div>
        ))}
      </div>

      <div className="simple-calendar-grid">
        {calendarCells.map((day, index) => {
          if (day === null) {
            return <div key={`empty-${index}`} className="simple-calendar-cell empty" />;
          }

          const dateKey = formatDateKey(year, month, day);
          const isBooked = bookedDates.includes(dateKey);
          const isSelected = selectedDate === dateKey;

          const today = new Date();
          today.setHours(0, 0, 0, 0);

          const currentCellDate = new Date(year, month, day);
          currentCellDate.setHours(0, 0, 0, 0);

          const isPast = currentCellDate < today;

          return (
            <button
              key={dateKey}
              className={`simple-calendar-cell day ${
                isBooked ? "booked" : ""
              } ${isSelected ? "selected" : ""} ${isPast ? "past" : ""}`}
              disabled={isBooked || isPast}
              onClick={() => onSelectDate(dateKey)}
            >
              {day}
            </button>
          );
        })}
      </div>
    </div>
  );
}