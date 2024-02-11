$(document).ready(function () {
  const tour = new Shepherd.Tour(AdminTourCommonTourOptions);

  AdminTourNextPageButton.action = function () { window.location = '/Admin/EmailAccount/List?showtour=True' };

  //'Settings button' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TvChannelSettingsButtonTitle,
    text: AdminTourDataProvider.localized_data.TvChannelSettingsButtonText,
    attachTo: {
      element: '#tvchannel-editor-settings',
      on: 'bottom'
    },
    buttons: [AdminTourNextButton]
  });

  //'TvChannel details' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TvChannelDetailsTitle,
    text: AdminTourDataProvider.localized_data.TvChannelDetailsText,
    attachTo: {
      element: '#tvchannel-details-area',
      on: 'bottom'
    },
    classes: 'step-with-image',
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'TvChannel price' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TvChannelPriceTitle,
    text: AdminTourDataProvider.localized_data.TvChannelPriceText,
    attachTo: {
      element: '#tvchannel-price-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'TvChannel tax category' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TvChannelTaxTitle,
    text: AdminTourDataProvider.localized_data.TvChannelTaxText,
    attachTo: {
      element: '#tvchannel-tax-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'TvChannel shipping info' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TvChannelShippingTitle,
    text: AdminTourDataProvider.localized_data.TvChannelShippingText,
    attachTo: {
      element: '#tvchannel-shipping-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'TvChannel inventory' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TvChannelInventoryTitle,
    text: AdminTourDataProvider.localized_data.TvChannelInventoryText,
    attachTo: {
      element: '#tvchannel-inventory-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextButton]
  });

  //'TvChannel pictures' step
  tour.addStep({
    title: AdminTourDataProvider.localized_data.TvChannelPicturesTitle,
    text: AdminTourDataProvider.localized_data.TvChannelPicturesText,
    attachTo: {
      element: '#tvchannel-pictures-area',
      on: 'bottom'
    },
    buttons: [AdminTourBackButton, AdminTourNextPageButton]
  });

  tour.start();
})