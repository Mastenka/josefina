jQuery(function ($) {
    $.datepicker.regional['cs'] = {
        closeText: 'Zavřít',
        prevText: 'Předchozí',
        nextText: 'Další',
        currentText: 'Dnes',
        monthNames: ['Leden', 'Únor', 'Březen', 'Duben', 'Květen', 'Červen', 'Červenec', 'Srpen', 'Září', 'Říjen', 'Listopad', 'Prosinec'],
        monthNamesShort: ['Led', 'Úno', 'Bře', 'Dub', 'Kvě', 'Čer', 'Čvc', 'Srp', 'Zář', 'Říj', 'Lis', 'Pro'],
        dayNames: ['Neděle', 'Pondělí', 'Úterý', 'Středa', 'Čtvrtek', 'Pátek', 'Sobota'],
        dayNamesShort: ['Ne', 'Po', 'Út', 'St', 'Čt', 'Pá', 'So', ],
        dayNamesMin: ['Ne', 'Po', 'Út', 'St', 'Čt', 'Pá', 'So'],
        weekHeader: 'Týd',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['cs']);
});