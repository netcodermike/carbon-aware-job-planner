def operation(data, score):
    deadline = data["deadlineHour"]
    if type(deadline) is not int or deadline < 0:
        raise ValueError("deadlineHour must be a nonnegative integer")
    inputs = {key: data[key] for key in ("gridGPerKwh", "hours", "kwhPerHour")}
    windows = [window for window in score(inputs)["windows"] if window["endHour"] <= deadline]
    best = min(windows, key=lambda window: (window["grams"], window["startHour"]), default=None)
    return {"eligibleWindows": len(windows), **(best or {"startHour": None, "endHour": None, "grams": None})}
