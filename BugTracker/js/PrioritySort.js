$.each($('td.ticket-priority'), function () {
    // cheesy script to order ticket priority
    switch ($(this).html().trim()) {
        case "Urgent":
            $(this).text("4 - Urgent");
            break;
        case "High":
            $(this).text("3 - High");
            break;
        case "Medium":
            $(this).text("2 - Medium");
            break;
        case "Low":
            $(this).text("1 - Low");
            break;
        case "Default":
            $(this).text("0 - Default");
    }

});