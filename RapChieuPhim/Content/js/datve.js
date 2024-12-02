function bookTicket(time, totalSeats, availableSeats) {
    const modal = document.getElementById("bookingModal");
    const showtimeInfo = document.getElementById("showtimeInfo");
    const seatsInput = document.getElementById("seats");

    showtimeInfo.innerHTML = `Suất chiếu: ${time}<br>Ghế còn lại: ${availableSeats}/${totalSeats}`;
    modal.style.display = "block";

    const form = document.getElementById("bookingForm");
    form.onsubmit = function(event) {
        event.preventDefault();
        const seats = parseInt(seatsInput.value);
        if (seats > availableSeats) {
            alert("Không đủ ghế trống.");
        } else {
            alert("Đặt vé thành công!");
            modal.style.display = "none";
        }
    };
}

document.querySelector(".close").onclick = function() {
    document.getElementById("bookingModal").style.display = "none";
};

window.onclick = function(event) {
    const modal = document.getElementById("bookingModal");
    if (event.target == modal) {
        modal.style.display = "none";
    }
};
document.addEventListener('DOMContentLoaded', () => {
    const seats = document.querySelectorAll('.seat:not(.occupied)');
    const confirmButton = document.getElementById('confirmSeats');
    
    seats.forEach(seat => {
        seat.addEventListener('click', () => {
            if (!seat.classList.contains('occupied')) {
                seat.classList.toggle('selected');
            }
        });
    });

    confirmButton.addEventListener('click', () => {
        const selectedSeats = document.querySelectorAll('.seat.selected');
        const seatNumbers = Array.from(selectedSeats).map(seat => {
            const row = seat.parentElement.getAttribute('data-row');
            const number = seat.getAttribute('data-seat');
            return row + number;
        });
        alert(`Bạn đã chọn các ghế: ${seatNumbers.join(', ')}`);
    });
});
