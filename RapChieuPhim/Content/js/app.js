var username = document.querySelector('#username')
var email = document.querySelector('#email')
var password = document.querySelector('#password')
var confirmPassword = document.querySelector('#confirm-password')
var form = document.querySelector('form')


function showError(input, message) {
    let parent = input.parentElement;
    let small = parent.querySelector('small')
    parent.classList.add('error')
    small.innerText = message
}

function showSuccess(input) {
    let parent = input.parentElement;
    let small = parent.querySelector('small')
    parent.classList.remove('error')
    small.innerText = ''
}


function checkEmptyError(listInput) {
    let isEmptyError = false;
    listInput.forEach(input => {
        input.value = input.value.trim()

        if (!input.value) {
            isEmptyError = true;
            showError(input, 'Vui lòng nhập mật khẩu')
        } else {
            showSuccess(input)
        }
    });
    return isEmptyError

}
function checkEmail(input) {
    var regexEmail = /^\s*[\w\-\+_]+(\.[\w\-\+_]+)*\@[\w\-\+_]+\.[\w\-\+_]+(\.[\w\-\+_]+)*\s*$/;
    input.value = input.value.trim()

    let isEmailError = !regexEmail.test(input.value)
    if (regexEmail.test(input.value)) {
        showSuccess()
    } else {
        showError(input, 'Email không hợp lệ    ')
    }

    return isEmailError
}
function checkLengthError(input, min, max) {
    input.value = input.value.trim()

    if (input.value.length < min) {
        showError(input, 'Phải có ít nhất 8 ký tự')
        return true
    }
    if (input.value.length > max) {
        showError(input, 'Không được vượt quá 10 ký tự')
        return true
    }
    return false
}
function checkMatchPassword(passwordInput, confirmPasswordInput) {
    if (passwordInput.value !== confirmPasswordInput.value) {
        showError(confirmPasswordInput, 'Mật khẩu không trùng khớp')
        return true
    }
    return false
}
form.addEventListener('submit', function (e) {
    e.preventDefault()
    let isEmptyError = checkEmptyError([username, email, password, confirmPassword])
    let isEmailError = checkEmail(email)
    let isUsernameLengthError = checkLengthError(username, 8, 10)
    let isPasswordLengthError = checkLengthError(password, 8, 10)
    let isMatchError = checkMatchPassword(password, confirmPassword)
    if (isEmptyError || isEmailError || isUsernameLengthError || isPasswordLengthError || isMatchError) {
        // do nothing
    } else {
        // logic, call API
    }
})
