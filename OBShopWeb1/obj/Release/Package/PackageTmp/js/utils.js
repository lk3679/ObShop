
var disableForm_flag = false;

function disableForm(theform)
{
  if(disableForm_flag)
  {
    return false;
  }
  else
  {
    disableForm_flag = true;

    for(i = 0; i < theform.length; i++)
    {
      var o = theform.elements[i];
      if(o.type.toLowerCase() == "submit")
        o.disabled = true;
    }

    return true;
  }
}
